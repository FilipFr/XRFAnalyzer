from google.protobuf.internal import containers as _containers
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from typing import ClassVar as _ClassVar, Iterable as _Iterable, Mapping as _Mapping, Optional as _Optional, Union as _Union

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
    __slots__ = ["counts", "distance", "height", "prominence", "wlen"]
    COUNTS_FIELD_NUMBER: _ClassVar[int]
    DISTANCE_FIELD_NUMBER: _ClassVar[int]
    HEIGHT_FIELD_NUMBER: _ClassVar[int]
    PROMINENCE_FIELD_NUMBER: _ClassVar[int]
    WLEN_FIELD_NUMBER: _ClassVar[int]
    counts: _containers.RepeatedScalarFieldContainer[float]
    distance: int
    height: int
    prominence: int
    wlen: int
    def __init__(self, counts: _Optional[_Iterable[float]] = ..., height: _Optional[int] = ..., distance: _Optional[int] = ..., prominence: _Optional[int] = ..., wlen: _Optional[int] = ...) -> None: ...

class NumericalData(_message.Message):
    __slots__ = ["data"]
    DATA_FIELD_NUMBER: _ClassVar[int]
    data: _containers.RepeatedScalarFieldContainer[float]
    def __init__(self, data: _Optional[_Iterable[float]] = ...) -> None: ...

class QuantificationReply(_message.Message):
    __slots__ = ["concentrations"]
    CONCENTRATIONS_FIELD_NUMBER: _ClassVar[int]
    concentrations: _containers.RepeatedScalarFieldContainer[float]
    def __init__(self, concentrations: _Optional[_Iterable[float]] = ...) -> None: ...

class QuantificationRequest(_message.Message):
    __slots__ = ["absorption_data", "attenuation_data", "coefficient_energies", "detector_efficiencies", "detector_energies", "intervals_per_channel", "jump_ratios", "p_counts", "p_intercept", "p_slope", "peak_areas", "peak_energies", "probabilities", "yields"]
    ABSORPTION_DATA_FIELD_NUMBER: _ClassVar[int]
    ATTENUATION_DATA_FIELD_NUMBER: _ClassVar[int]
    COEFFICIENT_ENERGIES_FIELD_NUMBER: _ClassVar[int]
    DETECTOR_EFFICIENCIES_FIELD_NUMBER: _ClassVar[int]
    DETECTOR_ENERGIES_FIELD_NUMBER: _ClassVar[int]
    INTERVALS_PER_CHANNEL_FIELD_NUMBER: _ClassVar[int]
    JUMP_RATIOS_FIELD_NUMBER: _ClassVar[int]
    PEAK_AREAS_FIELD_NUMBER: _ClassVar[int]
    PEAK_ENERGIES_FIELD_NUMBER: _ClassVar[int]
    PROBABILITIES_FIELD_NUMBER: _ClassVar[int]
    P_COUNTS_FIELD_NUMBER: _ClassVar[int]
    P_INTERCEPT_FIELD_NUMBER: _ClassVar[int]
    P_SLOPE_FIELD_NUMBER: _ClassVar[int]
    YIELDS_FIELD_NUMBER: _ClassVar[int]
    absorption_data: _containers.RepeatedCompositeFieldContainer[NumericalData]
    attenuation_data: _containers.RepeatedCompositeFieldContainer[NumericalData]
    coefficient_energies: _containers.RepeatedCompositeFieldContainer[NumericalData]
    detector_efficiencies: _containers.RepeatedScalarFieldContainer[float]
    detector_energies: _containers.RepeatedScalarFieldContainer[float]
    intervals_per_channel: int
    jump_ratios: _containers.RepeatedScalarFieldContainer[float]
    p_counts: _containers.RepeatedScalarFieldContainer[float]
    p_intercept: float
    p_slope: float
    peak_areas: _containers.RepeatedScalarFieldContainer[float]
    peak_energies: _containers.RepeatedScalarFieldContainer[float]
    probabilities: _containers.RepeatedScalarFieldContainer[float]
    yields: _containers.RepeatedScalarFieldContainer[float]
    def __init__(self, p_counts: _Optional[_Iterable[float]] = ..., intervals_per_channel: _Optional[int] = ..., p_slope: _Optional[float] = ..., p_intercept: _Optional[float] = ..., peak_areas: _Optional[_Iterable[float]] = ..., peak_energies: _Optional[_Iterable[float]] = ..., detector_energies: _Optional[_Iterable[float]] = ..., detector_efficiencies: _Optional[_Iterable[float]] = ..., yields: _Optional[_Iterable[float]] = ..., probabilities: _Optional[_Iterable[float]] = ..., jump_ratios: _Optional[_Iterable[float]] = ..., coefficient_energies: _Optional[_Iterable[_Union[NumericalData, _Mapping]]] = ..., absorption_data: _Optional[_Iterable[_Union[NumericalData, _Mapping]]] = ..., attenuation_data: _Optional[_Iterable[_Union[NumericalData, _Mapping]]] = ...) -> None: ...
