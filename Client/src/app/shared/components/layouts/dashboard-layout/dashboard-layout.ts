import {
  Component,
  signal,
  computed,
  OnInit,
  OnDestroy,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';

import { Roles } from '@/app/shared/constants/roles';
import { Sidebar } from '../../sidebar/sidebar';
import { ThemeToggle } from '../../ui/theme-toggle/theme-toggle';
import { AuthStateService } from '@/app/store/AuthStateService';
import { ProfileDropdown } from '../../profile-dropdown/profile-dropdown';

@Component({
  selector: 'app-dashboard-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, Sidebar, ThemeToggle, ProfileDropdown],
  templateUrl: './dashboard-layout.html',
  styleUrls: ['./dashboard-layout.css'],
})
export class DashboardLayout implements OnInit, OnDestroy {
  private router = inject(Router);
  private authState = inject(AuthStateService);

  sidebarOpen = signal(window.innerWidth > 768);
  pageTitle = signal('Dashboard');
  userRole = computed<Roles>(() => {
    var role = this.authState.user()?.role as Roles;
    return role ?? Roles.Organizer;
  });

  ngOnInit() {
    window.addEventListener('resize', this.handleResize);
    const currentPath = this.router.url.split('/').pop() || 'dashboard';
    this.pageTitle.set(this.formatPageTitle(currentPath));
  }

  ngOnDestroy() {
    window.removeEventListener('resize', this.handleResize);
  }

  formatPageTitle(path: string): string {
    path = path.split('?')[0].split('#')[0];

    return path
      .replace(/-/g, ' ')
      .split(' ')
      .map((word) => word.charAt(0).toUpperCase() + word.slice(1))
      .join(' ');
  }

  handleResize = () => {
    this.sidebarOpen.set(window.innerWidth > 768);
  };

  toggleSidebar() {
    this.sidebarOpen.update((prev) => !prev);
  }
}
