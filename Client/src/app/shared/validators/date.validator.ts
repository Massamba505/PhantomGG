import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function dateNotInPastValidator(
  control: AbstractControl
): ValidationErrors | null {
  if (!control.value) return null;

  const selectedDate = new Date(control.value);
  const today = new Date();
  today.setHours(0, 0, 0, 0);

  if (selectedDate < today) {
    return { dateInPast: true };
  }

  return null;
}

export function registrationDeadlineValidator(
  control: AbstractControl
): ValidationErrors | null {
  if (!control.parent) return null;

  const registrationStart = control.parent.get('registrationStartDate')?.value;
  const registrationDeadline = control.value;
  const startDate = control.parent.get('startDate')?.value;

  if (!registrationDeadline) return null;

  if (
    registrationStart &&
    new Date(registrationDeadline) <= new Date(registrationStart)
  ) {
    return { deadlineBeforeStart: true };
  }

  if (startDate && new Date(registrationDeadline) >= new Date(startDate)) {
    return { deadlineAfterTournamentStart: true };
  }

  return null;
}

export function tournamentStartDateValidator(
  control: AbstractControl
): ValidationErrors | null {
  if (!control.parent) return null;

  const registrationDeadline = control.parent.get(
    'registrationDeadline'
  )?.value;
  const startDate = control.value;

  if (!startDate) return null;
  if (
    registrationDeadline &&
    new Date(startDate) <= new Date(registrationDeadline)
  ) {
    return { startBeforeDeadline: true };
  }

  return null;
}

export function tournamentDatesValidator(): ValidatorFn {
  return (group: AbstractControl): ValidationErrors | null => {
    if (!group) return null;

    const regStart = new Date(group.get('registrationStartDate')?.value);
    const regDeadline = new Date(group.get('registrationDeadline')?.value);
    const startDate = new Date(group.get('startDate')?.value);
    const endDate = new Date(group.get('endDate')?.value);

    if (!(regStart && regDeadline && startDate && endDate)) return null;

    const errors: Record<string, boolean> = {};
    const now = new Date();

    if (regStart < now) {
      errors['registrationStartInPast'] = true;
    }

    if (regDeadline < regStart) {
      errors['registrationDeadlineBeforeStart'] = true;
    }

    if (startDate < regDeadline) {
      errors['startBeforeRegistrationDeadline'] = true;
    }

    if (endDate < startDate) {
      errors['endBeforeStart'] = true;
    }

    return Object.keys(errors).length ? errors : null;
  };
}
