import { Component, Input, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthStateService } from '@/app/store/AuthStateService';
import { ProfileDropdown } from '../profile-dropdown/profile-dropdown';
import { ThemeToggleComponent } from '../ui/theme-toggle/theme-toggle';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule, ProfileDropdown, ThemeToggleComponent],
  templateUrl: './footer.html',
})
export class FooterComponent {
  @Input() isOpen = true;

  private auth = inject(AuthStateService);
  user = signal('hello');
}
