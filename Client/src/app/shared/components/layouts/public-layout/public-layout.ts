import { Component, signal, computed, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ActivatedRoute,
  Router,
  RouterModule,
  NavigationEnd,
} from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { ThemeToggle } from '../../ui/theme-toggle/theme-toggle';
import { LucideIcons } from '../../ui/icons/lucide-icons';
import { filter, map } from 'rxjs';
import { AuthStateService } from '@/app/store/AuthStateService';

@Component({
  selector: 'app-public-layout',
  imports: [CommonModule, RouterModule, LucideAngularModule, ThemeToggle],
  templateUrl: './public-layout.html',
  styleUrls: ['./public-layout.css'],
})
export class PublicLayout implements OnInit {
  readonly router = inject(Router);
  private route = inject(ActivatedRoute);
  private readonly authState = inject(AuthStateService);
  isAuthenticated = signal(this.authState.isAuthenticated());

  readonly icons = LucideIcons;
  pageTitle = signal('PhantomGG');

  navigationItems = [
    {
      title: 'Home',
      url: '/',
      icon: this.icons.Home,
    },
    {
      title: 'Browse Tournaments',
      url: '/public/tournaments',
      icon: this.icons.Trophy,
    },
  ];

  ngOnInit() {
    this.router.events
      .pipe(
        filter((event) => event instanceof NavigationEnd),
        map(() => this.route.snapshot.firstChild?.title || 'PhantomGG')
      )
      .subscribe((title) => {
        this.pageTitle.set(title);
      });
  }

  navigateTo(url: string) {
    this.router.navigate([url]);
  }

  goToAuth() {
    this.router.navigate(['/auth/login']);
  }
}
