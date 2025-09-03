import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function strictEmailValidator(
  control: AbstractControl
): ValidationErrors | null {
  const email = control.value;

  const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

  return email && !emailRegex.test(email) ? { invalidEmail: true } : null;
}
