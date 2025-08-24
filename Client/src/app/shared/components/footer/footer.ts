import { Component, Input, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthStateService } from '@/app/store/AuthStateService';
import { ProfileDropdown } from '../profile-dropdown/profile-dropdown';
import { ThemeToggle } from '../ui/theme-toggle/theme-toggle';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule, ProfileDropdown, ThemeToggle],
  templateUrl: './footer.html',
})
export class Footer {
  @Input() isOpen = true;

  private auth = inject(AuthStateService);
  user = signal('hello');
}
