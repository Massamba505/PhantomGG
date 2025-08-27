import {
  Component,
  ElementRef,
  HostListener,
  inject,
  signal,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

import {
  LucideAngularModule,
} from 'lucide-angular';

import { AuthStateService } from '@/app/store/AuthStateService';
import { LucideIcons } from '../ui/icons/lucide-icons';

@Component({
  selector: 'app-profile-dropdown',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
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

  navigateTo(path: string) {
    this.closeDropdown();
    this.router.navigate([path]);
  }

  logout() {
    this.closeDropdown();
    this.authService.logout();
    this.router.navigate(['/']);
  }

  @HostListener('document:mousedown', ['$event'])
  handleClickOutside(event: Event) {
    if (!this.el.nativeElement.contains(event.target)) {
      this.closeDropdown();
    }
  }
}
