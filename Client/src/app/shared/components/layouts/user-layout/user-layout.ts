import {
  Component,
  signal,
  computed,
  OnInit,
  OnDestroy,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';

import { Roles } from '@/app/shared/constants/roles';
import { Sidebar } from '../../sidebar/sidebar';
import { ThemeToggle } from '../../ui/theme-toggle/theme-toggle';
import { AuthStateService } from '@/app/store/AuthStateService';
import { ProfileDropdown } from '../../profile-dropdown/profile-dropdown';
import { map } from 'rxjs';

@Component({
  selector: 'app-user-layout',
  imports: [CommonModule, RouterModule, Sidebar, ThemeToggle, ProfileDropdown],
  templateUrl: './user-layout.html',
  styleUrls: ['./user-layout.css'],
})
export class UserLayout implements OnInit, OnDestroy {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private authState = inject(AuthStateService);

  sidebarOpen = signal(window.innerWidth > 768);
  pageTitle = signal('Dashboard');
  userRole = computed<Roles>(() => {
    const role = this.authState.user()?.role as Roles;
    return role ?? Roles.Organizer;
  });

  ngOnInit() {
    window.addEventListener('resize', this.handleResize);

    this.router.events
      .pipe(
        map(() => {
          let route = this.route;
          while (route.firstChild) {
            route = route.firstChild;
          }
          return route;
        }),
        map((route) => route.snapshot.data['title'])
      )
      .subscribe((title: string | undefined) => {
        this.pageTitle.set(title ?? 'Dashboard');
      });
  }

  ngOnDestroy() {
    window.removeEventListener('resize', this.handleResize);
  }

  handleResize = () => {
    this.sidebarOpen.set(window.innerWidth > 768);
  };

  toggleSidebar() {
    this.sidebarOpen.update((prev) => !prev);
  }
}
