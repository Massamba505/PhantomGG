import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { Team } from '../models/tournament';

/**
 * Validator to check if a team name is unique within a tournament
 */
export function uniqueTeamNameValidator(existingTeams: Team[], currentTeamId?: string): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return null; // Let required validator handle empty values
    }
    
    const teamName = control.value.trim().toLowerCase();
    const nameExists = existingTeams.some(team => {
      // Skip the current team when editing
      if (currentTeamId && team.id === currentTeamId) {
        return false;
      }
      return team.name.trim().toLowerCase() === teamName;
    });
    
    return nameExists ? { uniqueName: true } : null;
  };
}

/**
 * Validator to ensure player count is within sport-specific limits
 */
export function playerCountValidator(min: number, max: number): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value && control.value !== 0) {
      return null; // Let required validator handle empty values
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
