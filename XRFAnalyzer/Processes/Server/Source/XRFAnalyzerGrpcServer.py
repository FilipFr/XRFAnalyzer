from concurrent import futures
import logging
import grpc
import XRFAnalyzer_pb2
import XRFAnalyzer_pb2_grpc
from scipy.signal import find_peaks
from sklearn import metrics
from BaselineRemoval import BaselineRemoval
from xrf_quantitative import quantitative_analysis, quantitative_analysis_mono


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
        distance = None if (request.distance <= 0) else request.distance
        prominence = None if (request.prominence <= 0) else request.prominence
        wlen = None if (request.wlen <= 0) else request.wlen

        peaks, left_bases, right_bases, p = get_peaks(data, height=height, distance=distance,
                                                      prominence=prominence, wlen=wlen)

        return XRFAnalyzer_pb2.FindPeaksReply(peaks=peaks, left_bases=left_bases, right_bases=right_bases)

    def BackgroundMessage(self, request, context):
        data = request.counts
        lambda_ = 100 if (request.lambda_ <= 0) else request.lambda_
        iterations = 15 if (request.iterations <= 0) else request.iterations

        if bool(data):
            data = get_corrected(data, lambda_, iterations)
        return XRFAnalyzer_pb2.BackgroundReply(corrected_counts=data)

    def QuantificationMessage(self, request, context):
        coefficient_energies = []
        absorption_data = []
        attenuation_data = []

        for i in request.coefficient_energies:
            values = []
            for j in i.data:
                values.append(j)
            coefficient_energies.append(values)
        for i in request.absorption_data:
            values = []
            for j in i.data:
                values.append(j)
            absorption_data.append(values)
        for i in request.attenuation_data:
            values = []
            for j in i.data:
                values.append(j)
            attenuation_data.append(values)

        if len(request.p_counts) <= 2:
            concentrations = quantitative_analysis_mono(request.p_counts[0], request.peak_areas, request.peak_energies,
                                  request.detector_energies, request.detector_efficiencies,
                                  request.yields, request.probabilities, request.jump_ratios,
                                  coefficient_energies, absorption_data, attenuation_data)
            return XRFAnalyzer_pb2.QuantificationReply(concentrations=concentrations)

        concentrations = \
            quantitative_analysis(request.p_counts, request.intervals_per_channel, request.p_slope,
                                  request.p_intercept, request.peak_areas, request.peak_energies,
                                  request.detector_energies, request.detector_efficiencies,
                                  request.yields, request.probabilities, request.jump_ratios,
                                  coefficient_energies, absorption_data, attenuation_data)
        return XRFAnalyzer_pb2.QuantificationReply(concentrations=concentrations)


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
