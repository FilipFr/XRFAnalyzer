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

MAX_ITERATIONS = 10


def quantitative_analysis(p_counts, intervals_per_channel, slope, intercept,
                          peak_areas, line_energies, detector_energies, detector_efficiencies,
                          yields, probabilities, jump_ratios,
                          coefficient_energies, absorption_data, attenuation_data):

    integral_bounds = get_integral_bounds(p_counts, intervals_per_channel, slope, intercept)

    p_energies = [x for x in range(0, len(integral_bounds), intervals_per_channel)]

    primary_spectrum_spline = make_interp_spline(p_energies, p_counts, k=1)

    p_integrals = get_integrals(integral_bounds, primary_spectrum_spline)

    coefficient_edges = locate_coefficient_edges(coefficient_energies)

    p_i_absorption_coefficients, p_j_attenuation_coefficients, i_j_attenuation_coefficients = \
        get_coefficients(integral_bounds, line_energies, coefficient_energies, coefficient_edges,
                         absorption_data, attenuation_data)
    normalized_peak_areas = normalize(peak_areas)

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


def locate_coefficient_edges(coefficient_energies):
    """
    Applies the function locate_edge to all elements of lists and returns a list of integers.

    :param list[list[float]] coefficient_energies: List of lists of sorted numerical data
    :return list[int]: List of integers
    """
    coefficient_edges = []
    for value in coefficient_energies:
        coefficient_edges.append(locate_edge(value))
    return coefficient_edges


def get_coefficients(integral_bounds, line_energies,
                     coefficient_energies, coefficient_edges, absorption_data, attenuation_data):

    p_i_absorption_coefficients = []
    p_j_attenuation_coefficients = []
    i_j_attenuation_coefficients = []

    attenuation_lsplines = []
    attenuation_rsplines = []

    for i in range(len(coefficient_edges)):
        edge = coefficient_edges[i]

        absorption_lspline = \
            make_interp_spline(coefficient_energies[i][:edge], absorption_data[i][:edge], k=1)
        absorption_rspline = \
            make_interp_spline(coefficient_energies[i][edge + 1:], absorption_data[i][edge + 1:], k=1)
        attenuation_lspline = \
            make_interp_spline(coefficient_energies[i][:edge], attenuation_data[i][:edge], k=1)
        attenuation_rspline = \
            make_interp_spline(coefficient_energies[i][edge + 1:], attenuation_data[i][edge + 1:], k=1)

        attenuation_lsplines.append(attenuation_lspline)
        attenuation_rsplines.append(attenuation_rspline)

        p_i_absorption_row = []
        p_j_attenuation_row = []

        for k in range(len(integral_bounds)):
            average = (integral_bounds[k] + integral_bounds[k+1]) / 2
            if average < coefficient_energies[i][edge]:
                p_i_absorption_row.append(float(splev(average, absorption_lspline)))
                p_j_attenuation_row.append(float(splev(average, attenuation_lspline)))
            else:
                p_i_absorption_row.append(float(splev(average, absorption_rspline)))
                p_j_attenuation_row.append(float(splev(average, attenuation_rspline)))
        p_i_absorption_coefficients.append(p_i_absorption_row)
        p_j_attenuation_coefficients.append(p_j_attenuation_row)

    for i in range(len(line_energies)):
        i_j_attenuation_row = []
        for j in range(len(attenuation_lsplines)):
            if line_energies[i] < coefficient_energies[j][coefficient_edges[j]]:
                i_j_attenuation_row.append(float(splev(line_energies[i], attenuation_lsplines[j])))
            else:
                i_j_attenuation_row.append(float(splev(line_energies[i], attenuation_rsplines[j])))
        i_j_attenuation_coefficients.append(i_j_attenuation_row)

    return p_i_absorption_coefficients, p_j_attenuation_coefficients, i_j_attenuation_coefficients


def calculate_concentrations(previous_result, peak_areas, detector, yields, probabilities, jump_ratios, p_integrals,
                             p_i_absorption_coefficients, p_j_attenuation_coefficients, i_j_attenuation_coefficients,
                             current_iteration=0):
    if current_iteration >= MAX_ITERATIONS:
        return previous_result

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

    return calculate_concentrations(concentrations_normalized, peak_areas, detector, yields, probabilities, jump_ratios,
                                    p_integrals, p_i_absorption_coefficients, p_j_attenuation_coefficients,
                                    i_j_attenuation_coefficients, current_iteration=next_iteration)

def get_value_from_spline(a, x_values, y_values):
    if len(x_values) != len(y_values):
        raise ValueError("Dataset lengths do not match")
    if a < x_values[0] or a > x_values[-1]:
        raise ValueError("Value out of range bounds")
    left_edge_index = locate_coefficient_edges(x_values)[0]
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