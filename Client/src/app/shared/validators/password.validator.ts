import { AbstractControl, ValidationErrors } from '@angular/forms';

export function passwordStrengthValidator(
  control: AbstractControl
): ValidationErrors | null {
  const value = control.value;
  if (!value) return null;

  const hasUpperCase = /[A-Z]/.test(value);
  const hasLowerCase = /[a-z]/.test(value);
  const hasNumeric = /[0-9]/.test(value);
  const hasSpecialChars = /[!@#$%^&*(),.?":{}|<>]/.test(value);
  const validLength = value.length >= 8;

  const isStrong =
    hasUpperCase &&
    hasLowerCase &&
    hasNumeric &&
    hasSpecialChars &&
    validLength;

  return isStrong ? null : { weakPassword: true };
}
