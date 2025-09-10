export class ValidationUtils {
  static isValidEmail(email: string | null | undefined): boolean {
    if (!email) return false;
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }

  static sanitizeString(input: string | null | undefined): string {
    if (!input) return '';
    return input.trim().replace(/[<>]/g, '');
  }

  static truncateString(input: string | null | undefined, maxLength: number): string {
    if (!input) return '';
    if (input.length <= maxLength) return input;
    return `${input.substring(0, maxLength - 3)}...`;
  }

  static isValidFileType(file: File, allowedTypes: string[]): boolean {
    return allowedTypes.includes(file.type);
  }

  static isValidFileSize(file: File, maxSizeInMB: number): boolean {
    const maxSizeInBytes = maxSizeInMB * 1024 * 1024;
    return file.size <= maxSizeInBytes;
  }

  static isValidImageFile(file: File, maxSizeInMB: number = 5): {
    isValid: boolean;
    error?: string;
  } {
    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif'];
    
    if (!this.isValidFileType(file, allowedTypes)) {
      return {
        isValid: false,
        error: 'Please select a valid image file (JPEG, PNG, or GIF)'
      };
    }
    
    if (!this.isValidFileSize(file, maxSizeInMB)) {
      return {
        isValid: false,
        error: `Image size must be less than ${maxSizeInMB}MB`
      };
    }
    
    return { isValid: true };
  }
}
