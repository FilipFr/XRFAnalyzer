from google.protobuf.internal import containers as _containers
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from typing import ClassVar as _ClassVar, Iterable as _Iterable, Optional as _Optional

DESCRIPTOR: _descriptor.FileDescriptor

class BackgroundReply(_message.Message):
    __slots__ = ["corrected_counts"]
    CORRECTED_COUNTS_FIELD_NUMBER: _ClassVar[int]
    corrected_counts: _containers.RepeatedScalarFieldContainer[float]
    def __init__(self, corrected_counts: _Optional[_Iterable[float]] = ...) -> None: ...

class BackgroundRequest(_message.Message):
    __slots__ = ["counts", "iterations", "lambda_"]
    COUNTS_FIELD_NUMBER: _ClassVar[int]
    ITERATIONS_FIELD_NUMBER: _ClassVar[int]
    LAMBDA__FIELD_NUMBER: _ClassVar[int]
    counts: _containers.RepeatedScalarFieldContainer[float]
    iterations: int
    lambda_: int
    def __init__(self, counts: _Optional[_Iterable[float]] = ..., lambda_: _Optional[int] = ..., iterations: _Optional[int] = ...) -> None: ...

class FindPeaksReply(_message.Message):
    __slots__ = ["left_bases", "peaks", "right_bases"]
    LEFT_BASES_FIELD_NUMBER: _ClassVar[int]
    PEAKS_FIELD_NUMBER: _ClassVar[int]
    RIGHT_BASES_FIELD_NUMBER: _ClassVar[int]
    left_bases: _containers.RepeatedScalarFieldContainer[int]
    peaks: _containers.RepeatedScalarFieldContainer[int]
    right_bases: _containers.RepeatedScalarFieldContainer[int]
    def __init__(self, peaks: _Optional[_Iterable[int]] = ..., left_bases: _Optional[_Iterable[int]] = ..., right_bases: _Optional[_Iterable[int]] = ...) -> None: ...

class FindPeaksRequest(_message.Message):
    __slots__ = ["counts", "distance", "height", "plateau_size", "prominence", "rel_height", "threshold", "width", "wlen"]
    COUNTS_FIELD_NUMBER: _ClassVar[int]
    DISTANCE_FIELD_NUMBER: _ClassVar[int]
    HEIGHT_FIELD_NUMBER: _ClassVar[int]
    PLATEAU_SIZE_FIELD_NUMBER: _ClassVar[int]
    PROMINENCE_FIELD_NUMBER: _ClassVar[int]
    REL_HEIGHT_FIELD_NUMBER: _ClassVar[int]
    THRESHOLD_FIELD_NUMBER: _ClassVar[int]
    WIDTH_FIELD_NUMBER: _ClassVar[int]
    WLEN_FIELD_NUMBER: _ClassVar[int]
    counts: _containers.RepeatedScalarFieldContainer[float]
    distance: int
    height: int
    plateau_size: int
    prominence: int
    rel_height: float
    threshold: int
    width: int
    wlen: int
    def __init__(self, counts: _Optional[_Iterable[float]] = ..., height: _Optional[int] = ..., threshold: _Optional[int] = ..., distance: _Optional[int] = ..., prominence: _Optional[int] = ..., width: _Optional[int] = ..., wlen: _Optional[int] = ..., rel_height: _Optional[float] = ..., plateau_size: _Optional[int] = ...) -> None: ...
