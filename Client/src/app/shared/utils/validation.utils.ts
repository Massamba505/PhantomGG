export class ValidationUtils {
  static isValidEmail(email: string | null | undefined): boolean {
    if (!email) return false;
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }

  static isValidUrl(url: string | null | undefined): boolean {
    if (!url) return true;
    try {
      new URL(url);
      return true;
    } catch {
      return false;
    }
  }

  static isValidPhoneNumber(phone: string | null | undefined): boolean {
    if (!phone) return true; 
    const phoneRegex = /^[\+]?[1-9][\d]{0,15}$/;
    return phoneRegex.test(phone.replace(/[\s\-\(\)]/g, ''));
  }

  static validatePassword(password: string): {
    isValid: boolean;
    errors: string[];
  } {
    const errors: string[] = [];
    
    if (password.length < 8) {
      errors.push('Password must be at least 8 characters long');
    }
    
    if (!/[A-Z]/.test(password)) {
      errors.push('Password must contain at least one uppercase letter');
    }
    
    if (!/[a-z]/.test(password)) {
      errors.push('Password must contain at least one lowercase letter');
    }
    
    if (!/[0-9]/.test(password)) {
      errors.push('Password must contain at least one number');
    }
    
    if (!/[^A-Za-z0-9]/.test(password)) {
      errors.push('Password must contain at least one special character');
    }
    
    return {
      isValid: errors.length === 0,
      errors
    };
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

  static isValidGuid(guid: string | null | undefined): boolean {
    if (!guid) return false;
    const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;
    return guidRegex.test(guid);
  }

  static isDateInFuture(date: Date | string | null | undefined): boolean {
    if (!date) return false;
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    return dateObj > new Date();
  }

  static isValidDateRange(startDate: Date | string, endDate: Date | string): boolean {
    const start = typeof startDate === 'string' ? new Date(startDate) : startDate;
    const end = typeof endDate === 'string' ? new Date(endDate) : endDate;
    return start < end;
  }

  static isInRange(value: number, min: number, max: number): boolean {
    return value >= min && value <= max;
  }

  static stripHtml(html: string): string {
    const doc = new DOMParser().parseFromString(html, 'text/html');
    return doc.body.textContent || '';
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
    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];
    
    if (!this.isValidFileType(file, allowedTypes)) {
      return {
        isValid: false,
        error: 'Please select a valid image file (JPEG, PNG, GIF, or WebP)'
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
