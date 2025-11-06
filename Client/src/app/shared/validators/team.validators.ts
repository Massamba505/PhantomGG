import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function playerCountValidator(min: number, max: number): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value && control.value !== 0) {
      return null;
    }

    const count = Number(control.value);
    if (isNaN(count)) {
      return { notANumber: true };
    }

    if (count < min) {
      return { minPlayers: { min, actual: count } };
    }

    if (count > max) {
      return { maxPlayers: { max, actual: count } };
    }

    return null;
  };
}
