import { Component, signal, computed, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';

import { Sidebar } from '../../sidebar/sidebar';
import { ThemeToggle } from '../../ui/theme-toggle/theme-toggle';
import { AuthStateService } from '@/app/store/AuthStateService';
import { ProfileDropdown } from '../../profile-dropdown/profile-dropdown';
import { map } from 'rxjs';
import { UserRoles } from '@/app/api/models';

@Component({
  selector: 'app-user-layout',
  imports: [CommonModule, RouterModule, Sidebar, ThemeToggle, ProfileDropdown],
  templateUrl: './user-layout.html',
  styleUrls: ['./user-layout.css'],
})
export class UserLayout implements OnInit {
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly authState = inject(AuthStateService);

  sidebarOpen = signal(true);
  pageTitle = signal('Dashboard');
  userRole = computed<UserRoles>(() => {
    const role = this.authState.user()?.role as UserRoles;
    return role ?? UserRoles.Organizer;
  });

  ngOnInit() {
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

  onSidebarStateChange(state: boolean) {
    this.sidebarOpen.set(state);
  }
}
