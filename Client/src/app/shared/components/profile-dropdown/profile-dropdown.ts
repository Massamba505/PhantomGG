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
  UserCircle2,
  Settings,
  LogOut,
} from 'lucide-angular';

import { AuthStateService } from '@/app/store/AuthStateService';

@Component({
  selector: 'app-profile-dropdown',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  templateUrl: './profile-dropdown.html',
})
export class ProfileDropdown {
  private router = inject(Router);
  private el = inject(ElementRef);
  private auth = inject(AuthStateService);

  open = signal(false);

  user = signal('hi');

  icon = [
    { UserCircle2: UserCircle2 },
    { Settings: Settings },
    { LogOut: LogOut },
  ];

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
    this.auth.logout();
    this.router.navigate(['/']);
  }

  @HostListener('document:mousedown', ['$event'])
  handleClickOutside(event: Event) {
    if (!this.el.nativeElement.contains(event.target)) {
      this.closeDropdown();
    }
  }
}
