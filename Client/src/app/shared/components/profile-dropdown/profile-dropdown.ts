import {
  Component,
  ElementRef,
  HostListener,
  inject,
  signal,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

import { LucideAngularModule } from 'lucide-angular';

import { AuthStateService } from '@/app/store/AuthStateService';
import { LucideIcons } from '../ui/icons/lucide-icons';

@Component({
  selector: 'app-profile-dropdown',
  imports: [CommonModule, LucideAngularModule, RouterLink],
  templateUrl: './profile-dropdown.html',
})
export class ProfileDropdown {
  private router = inject(Router);
  private el = inject(ElementRef);
  private authService = inject(AuthStateService);
  readonly user = this.authService.user()!;
  open = signal(false);

  readonly icons = LucideIcons;

  toggleDropdown() {
    this.open.update((prev) => !prev);
  }

  closeDropdown() {
    this.open.set(false);
  }

  logout() {
    this.closeDropdown();
    this.authService.logout().subscribe({
      next: () => {
        this.router.navigate(['/']);
      },
      error: (error) => {
        this.router.navigate(['/']);
      },
    });
  }

  @HostListener('document:mousedown', ['$event'])
  handleClickOutside(event: Event) {
    if (!this.el.nativeElement.contains(event.target)) {
      this.closeDropdown();
    }
  }
}
