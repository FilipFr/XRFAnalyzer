"""
XRF Quantitative Analysis Module

Uses fundamental parameter method to calculate concentrations of elements detected in a sample.
The method requires the following input data:
    - calibrated primary X-ray source spectrum (counts, calibration slope and intercept)
    - characteristic peak areas and energies with properly assigned emission lines
    - line dependent parameters (detector efficiency, yield, jump ratio, transition probabilities)
    - matrix effect coefficients (absorption, attenuation) at multiple energy levels

"""
import math
import numpy as np
from scipy.interpolate import make_interp_spline, splint, splev

MAX_ITERATIONS = 500

def quantitative_analysis_mono(energy, peak_areas, line_energies, detector_energies, detector_efficiencies,
                          yields, probabilities, jump_ratios,
                          coefficient_energies, absorption_data, attenuation_data):

    detector = []
    absorptions = []
    attenuations = []
    attenuation_splines = []
    ij = []
    for i in range(len(line_energies)):
        print(coefficient_energies)
        print(absorption_data[i])
        detector_en = remove_edges(detector_energies)
        detector_spline = make_interp_spline(detector_en, detector_efficiencies, k=1)
        detector.append(splev(energy, detector_spline))
        absorption_spline = make_interp_spline(remove_edges(coefficient_energies[i]), absorption_data[i], k=1)
        absorptions.append(splev(energy, absorption_spline))
        attenuation_spline = make_interp_spline(remove_edges(coefficient_energies[i]), attenuation_data[i], k=1)
        attenuations.append(splev(energy, attenuation_spline))
        attenuation_splines.append(attenuation_spline)
    for i in line_energies:
        ij_i = []
        for spline in attenuation_splines:
            ij_i.append(splev(i, spline))
        ij.append(ij_i)
    normalized_peak_areas = normalize(peak_areas)
    return calculate_concentrations_mono(normalized_peak_areas, peak_areas, detector, yields, probabilities, jump_ratios, [energy],
                                 absorptions, attenuations,
                                 ij,
                                 current_iteration=0)

def quantitative_analysis(p_counts, intervals_per_channel, slope, intercept,
                          peak_areas, line_energies, detector_energies, detector_efficiencies,
                          yields, probabilities, jump_ratios,
                          coefficient_energies, absorption_data, attenuation_data):

    integral_bounds = get_integral_bounds(p_counts, intervals_per_channel, slope, intercept)
    print("0")
    p_energies = [x * slope + intercept for x in range(0, len(p_counts))]
    print("a")
    primary_spectrum_spline = make_interp_spline(p_energies, p_counts, k=1)
    print("b")
    p_integrals = get_integrals(integral_bounds, primary_spectrum_spline)
    print("c")
    coefficient_edges = locate_coefficient_edges(coefficient_energies)
    print("d")
    p_i_absorption_coefficients, p_j_attenuation_coefficients, i_j_attenuation_coefficients = \
        get_coefficients(integral_bounds, line_energies, coefficient_energies,
                         absorption_data, attenuation_data)
    normalized_peak_areas = normalize(peak_areas)
    print("e")
    i_detector_efficiencies = []
    for energy in line_energies:
        i_detector_efficiencies.append(get_value_from_spline(energy, detector_energies, detector_efficiencies))

    return calculate_concentrations(normalized_peak_areas, peak_areas, i_detector_efficiencies,
                                    yields, probabilities, jump_ratios,
                                    p_integrals, p_i_absorption_coefficients, p_j_attenuation_coefficients,
                                    i_j_attenuation_coefficients)


def get_integral_bounds(source_counts, intervals_per_channel, slope, intercept):
    channel_bounds = np.linspace(0, len(source_counts) - 1,
                                 num=int((intervals_per_channel * (len(source_counts) - 1))))

    return [x * slope + intercept for x in range(len(channel_bounds))]


def get_integrals(integral_bounds, primary_spectrum_spline):
    integrals = []
    for i in range(len(integral_bounds) - 1):
        integrals.append(splint(integral_bounds[i], integral_bounds[i + 1], primary_spectrum_spline))
    return integrals


def normalize(values):
    normalized_values = []
    sum_of_values = sum(values)
    for value in values:
        normalized_values.append(float(value) / sum_of_values)
    return normalized_values


def locate_edge(x_values):
    """
    Returns the first index of a reoccurring value in a list of sorted numerical data.

    :param list x_values: List of sorted numerical data
    :return int i: First index of a reoccurring value
    """
    if len(x_values) > 1:
        for i in range(len(x_values)-1):
            if x_values[i] == x_values[i+1]:
                return i
    return -1

def locate_edges(x_values):
    edges = []
    if len(x_values) > 1:
        for i in range(len(x_values)-1):
            if x_values[i] == x_values[i+1]:
                edges.append(i)
    return edges

def remove_edges(x_values):
    values = [x_values[0]]
    e = 0.00001
    if len(x_values) > 1:
        for i in range(1, len(x_values)):
            if x_values[i - 1] == x_values[i]:
                values.append(x_values[i]+e)
                e += 0.00001
            else:
                values.append(x_values[i])
    values.sort()
    return values

def locate_coefficient_edges(coefficient_energies):
    """
    Applies the function locate_edge to all elements of lists and returns a list of integers.

    :param list[list[float]] coefficient_energies: List of lists of sorted numerical data
    :return list[int]: List of integers
    """
    coefficient_edges = []
    for value in coefficient_energies:
        coefficient_edges.append(locate_edges(value))
    return coefficient_edges


def get_coefficients(integral_bounds, line_energies,
                     coefficient_energies, absorption_data, attenuation_data):

    p_i_absorption_coefficients = []
    p_j_attenuation_coefficients = []
    i_j_attenuation_coefficients = []

    attenuation_splines = []


    for i in range(len(coefficient_energies)):
        energies = remove_edges(coefficient_energies[i])

        print(energies)
        print(absorption_data)
        absorption_spline = make_interp_spline(energies, absorption_data[i], k=1)
        attenuation_spline = make_interp_spline(energies, attenuation_data[i], k=1)

        attenuation_splines.append(attenuation_spline)

        p_i_absorption_row = []
        p_j_attenuation_row = []

        for k in range(len(integral_bounds)-1):
            average = (integral_bounds[k] + integral_bounds[k+1]) / 2
            p_i_absorption_row.append((float(splev(average, absorption_spline))))
            p_j_attenuation_row.append((float(splev(average, attenuation_spline))))

        p_i_absorption_coefficients.append(p_i_absorption_row)
        p_j_attenuation_coefficients.append(p_j_attenuation_row)

    for i in range(len(line_energies)):
        i_j_attenuation_row = []
        for j in range(len(attenuation_splines)):
            i_j_attenuation_row.append(float(splev(line_energies[i], attenuation_splines[j])))
        i_j_attenuation_coefficients.append(i_j_attenuation_row)

    return p_i_absorption_coefficients, p_j_attenuation_coefficients, i_j_attenuation_coefficients


def calculate_concentrations(previous_result, peak_areas, detector, yields, probabilities, jump_ratios, p_integrals,
                             p_i_absorption_coefficients, p_j_attenuation_coefficients, i_j_attenuation_coefficients,
                             current_iteration=0):

    if current_iteration >= MAX_ITERATIONS:
        return previous_result

    print(previous_result)
    if current_iteration == 0:
        previous_result = normalize(peak_areas)
    next_iteration = current_iteration + 1

    concentrations = []

    p_j_attenuation_sums = []
    i_j_attenuation_sums = []

    for i in range(len(previous_result)):
        value = 0
        for j in range(len(previous_result)):
            value += i_j_attenuation_coefficients[i][j] * previous_result[j]
        for j in range(len(p_integrals)):
            p_j_attenuation_sums_row = []
            for k in range(len(previous_result)):
                p_j_attenuation_sums_row.append(p_j_attenuation_coefficients[k][j] * previous_result[k])
            p_j_attenuation_sums.append(sum(p_j_attenuation_sums_row))
        i_j_attenuation_sums.append(value)

    for i in range(len(peak_areas)):
        constant_part = 1 / math.sqrt(2) * detector[i] * yields[i] * probabilities[i] * (1 - 1 / jump_ratios[i])
        calculated_intensities = []
        for j in range(len(p_integrals)):
            calculated_intensities.append(constant_part * p_i_absorption_coefficients[i][j] * p_integrals[j] /
                                          (p_j_attenuation_sums[j] + i_j_attenuation_sums[i]))
        concentrations.append(peak_areas[i] / sum(calculated_intensities))

    concentrations_normalized = normalize(concentrations)

    if current_iteration >= MAX_ITERATIONS:
        return concentrations_normalized
    diff = []
    for i in range(len(concentrations_normalized)):
        diff.append(concentrations_normalized[i] - previous_result[i])
    if stopping_criterion(diff, 0.000001):
        return concentrations_normalized
    return calculate_concentrations(concentrations_normalized, peak_areas, detector, yields, probabilities, jump_ratios,
                                    p_integrals, p_i_absorption_coefficients, p_j_attenuation_coefficients,
                                    i_j_attenuation_coefficients, current_iteration=next_iteration)

def calculate_concentrations_mono(previous_result, peak_areas, detector, yields, probabilities, jump_ratios, p_integrals,
                             p_i_absorption_coefficients, p_j_attenuation_coefficients, i_j_attenuation_coefficients,
                             current_iteration=0):

    if current_iteration >= MAX_ITERATIONS:
        return previous_result

    print(previous_result)
    if current_iteration == 0:
        previous_result = normalize(peak_areas)
    next_iteration = current_iteration + 1

    concentrations = []

    p_j_attenuation_sums = []
    i_j_attenuation_sums = []

    for i in range(len(previous_result)):
        value = 0
        for j in range(len(previous_result)):
            value += i_j_attenuation_coefficients[i][j] * previous_result[j]

        p_j_attenuation_sums_row = []
        for k in range(len(previous_result)):
            p_j_attenuation_sums_row.append(p_j_attenuation_coefficients[k] * previous_result[k])
        p_j_attenuation_sums.append(sum(p_j_attenuation_sums_row))
        i_j_attenuation_sums.append(value)

    for i in range(len(peak_areas)):
        constant_part = 1 / math.sqrt(2) * detector[i] * yields[i] * probabilities[i] * (1 - 1 / jump_ratios[i])

        calculated_intensities = constant_part * p_i_absorption_coefficients[i] * p_integrals[0] /\
                                 (p_j_attenuation_sums[0] + i_j_attenuation_sums[i])
        concentrations.append(peak_areas[i] / calculated_intensities)

    concentrations_normalized = normalize(concentrations)

    if current_iteration >= MAX_ITERATIONS:
        return concentrations_normalized
    diff = []
    for i in range(len(concentrations_normalized)):
        diff.append(concentrations_normalized[i] - previous_result[i])
    if stopping_criterion(diff, 0.000001):
        return concentrations_normalized
    return calculate_concentrations_mono(concentrations_normalized, peak_areas, detector, yields, probabilities, jump_ratios,
                                    p_integrals, p_i_absorption_coefficients, p_j_attenuation_coefficients,
                                    i_j_attenuation_coefficients, current_iteration=next_iteration)



def stopping_criterion(values, change_threshold):
    for value in values:
        if value > change_threshold:
            return False
    return True


def get_value_from_spline(a, x_values, y_values):
    if len(x_values) != len(y_values):
        raise ValueError("Dataset lengths do not match")
    if a < x_values[0] or a > x_values[-1]:
        raise ValueError("Value out of range bounds")
    left_edge_index = locate_edge(x_values)
    if left_edge_index == -1:
        linear_spline = make_interp_spline(x_values, y_values, k=1)
        return splev(a, linear_spline)
    else:
        if a < x_values[left_edge_index]:
            left_spline = make_interp_spline(x_values[0:left_edge_index + 1], y_values[0:left_edge_index + 1], k=1)
            return splev(a, left_spline)
        elif a >= x_values[left_edge_index]:
            if a == x_values[left_edge_index] and a + 0.01 > x_values[-1]:
                raise ValueError("Unable to approximate y for edge location x")
            if a == x_values[left_edge_index]:
                a += 0.01
            return get_value_from_spline(a, x_values[left_edge_index + 1:], y_values[left_edge_index + 1:])