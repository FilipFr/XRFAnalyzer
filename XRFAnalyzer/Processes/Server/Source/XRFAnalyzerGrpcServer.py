from concurrent import futures
import logging
import grpc
import XRFAnalyzer_pb2
import XRFAnalyzer_pb2_grpc
from scipy.signal import find_peaks
from sklearn import metrics
from BaselineRemoval import BaselineRemoval


def get_key(input, key):
    return input[key] if key in input else None


def get_peaks(data, height=None, threshold=None, distance=None,
              prominence=None, width=None, wlen=None, rel_height=None, plateau_size=None):
    peaks, properties = find_peaks(data, height, threshold, distance, prominence, width, wlen, rel_height,
                                   plateau_size)
    left_bases = get_key(properties, "left_bases")
    right_bases = get_key(properties, "right_bases")
    print(properties)
    return peaks.tolist(), left_bases, right_bases, properties


def get_corrected(data, lambda_=100, iterations=15):
    output = BaselineRemoval(data).ZhangFit(lambda_=lambda_, repitition=iterations)
    return output


class XRFAnalyzer(XRFAnalyzer_pb2_grpc.XRFAnalyzerServiceServicer):

    def FindPeaksMessage(self, request, context):
        data = request.counts
        height = None if (request.height <= 0) else request.height
        threshold = None if (request.threshold <= 0) else request.threshold
        distance = None if (request.distance <= 0) else request.distance
        prominence = None if (request.prominence <= 0) else request.prominence
        width = None if (request.width <= 0) else request.width
        wlen = None if (request.wlen <= 0) else request.wlen
        rel_height = None if (request.rel_height <= 0) else request.rel_height
        plateau_size = None if (request.plateau_size <= 0) else request.plateau_size

        peaks, left_bases, right_bases, p = get_peaks(data, height, threshold, distance,
                                                      prominence, width, wlen, rel_height, plateau_size)
        print(data)
        print(peaks)
        print(left_bases)
        print(right_bases)
        print(p)
        return XRFAnalyzer_pb2.FindPeaksReply(peaks=peaks, left_bases=left_bases, right_bases=right_bases)

    def BackgroundMessage(self, request, context):
        data = request.counts
        lambda_ = 100 if (request.lambda_ <= 0) else request.lambda_
        iterations = 15 if (request.iterations <= 0) else request.iterations

        if bool(data):
            data = get_corrected(data, lambda_, iterations)
        return XRFAnalyzer_pb2.BackgroundReply(corrected_counts=data)


def serve():
    port = '50051'
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    XRFAnalyzer_pb2_grpc.add_XRFAnalyzerServiceServicer_to_server(XRFAnalyzer(), server)
    server.add_insecure_port('[::]:' + port)
    server.start()
    print("Server started, listening on " + port)
    server.wait_for_termination()


if __name__ == '__main__':
    logging.basicConfig()
    serve()
