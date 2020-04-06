from mip import *
import typing


class ConstraintData(typing.NamedTuple):
    constraint: str
    result: int


def create_constraint(model: Model, digits_container: typing.List[typing.List[Var]], constraint: ConstraintData) -> None:
    digits_count = len(digits_container)
    digits_constraint = list(map(int, constraint.constraint))
    model += xsum(digits_container[digit][digits_constraint[digit]] for digit in range(digits_count)) == constraint.result


def extract_solution_digit(digit_container: typing.List[Var]) -> int:
    for digit in range(10):
        if digit_container[digit].x == 1:
            return digit
    raise ValueError('digit')


def solve(digits_count: int, constraints: typing.List[ConstraintData]) -> None:
    model = Model(sense=MAXIMIZE)
    digits = [[model.add_var(var_type=BINARY) for digit_index in range(10)] for digit in range(digits_count)]
    for constraint in constraints:
        create_constraint(model, digits, constraint)
    for digit in digits:
        model += xsum(digit[digit_index] for digit_index in range(10)) == 1
    model.objective = xsum(digits[digit][digit_index] for digit in range(digits_count) for digit_index in range(10))
    status = model.optimize()
    if status == OptimizationStatus.OPTIMAL:
        print('optimal solution cost {} found'.format(model.objective_value))
        print('solution = {0}'.format(''.join(map(lambda digit: str(extract_solution_digit(digit)), digits))))
    else:
        raise ValueError('source data')


if __name__ == "__main__":
    print('Solve for SAMPLE: ')
    sample_constraints = [ConstraintData('90342' ,2), ConstraintData('70794', 0), ConstraintData('39458', 2), ConstraintData('34109', 1), ConstraintData('51545', 2), ConstraintData('12531', 1)]
    solve(5, sample_constraints)
    print('')
    print('Solve for PROBLEM: ')
    problem_constraints = [ConstraintData('5616185650518293', 2),
                           ConstraintData('3847439647293047', 1),
                           ConstraintData('5855462940810587', 3),
                           ConstraintData('9742855507068353', 3),
                           ConstraintData('4296849643607543', 3),
                           ConstraintData('3174248439465858', 1),
                           ConstraintData('4513559094146117', 2),
                           ConstraintData('7890971548908067', 3),
                           ConstraintData('8157356344118483', 1),
                           ConstraintData('2615250744386899', 2),
                           ConstraintData('8690095851526254', 3),
                           ConstraintData('6375711915077050', 1),
                           ConstraintData('6913859173121360', 1),
                           ConstraintData('6442889055042768', 2),
                           ConstraintData('2321386104303845', 0),
                           ConstraintData('2326509471271448', 2),
                           ConstraintData('5251583379644322', 2),
                           ConstraintData('1748270476758276', 3),
                           ConstraintData('4895722652190306', 1),
                           ConstraintData('3041631117224635', 3),
                           ConstraintData('1841236454324589', 3),
                           ConstraintData('2659862637316867', 2)]
    solve(16, problem_constraints)
