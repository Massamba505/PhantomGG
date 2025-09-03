import {
  Component,
  input,
  output,
  OnInit,
  OnDestroy,
  signal,
  computed,
  effect,
} from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

import {
  LucideAngularModule,
  Home,
  Trophy,
  Users,
  Calendar,
  PlusCircle,
  Bell,
  MenuIcon,
  Settings,
  Briefcase,
  BarChart,
  Clock,
  HelpCircle,
} from 'lucide-angular';
import { Roles } from '../../constants/roles';

interface MenuItem {
  title: string;
  url: string;
  icon: any;
  roles: Roles[];
  badge?: string;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule, LucideAngularModule],
  templateUrl: './sidebar.html',
})
export class Sidebar implements OnInit, OnDestroy {
  isOpen = input<boolean>(true);
  userRole = input.required<Roles>();
  toggle = output<void>();

  private internalOpen = signal(true);
  
  actuallyOpen = computed(() => {
    if (typeof window !== 'undefined') {
      return window.innerWidth > 768 ? this.isOpen() : this.internalOpen();
    }
    return this.isOpen();
  });

  readonly MenuIcon = MenuIcon;
  menuItems: MenuItem[] = [
    {
      title: 'Dashboard',
      url: '/dashboard',
      icon: Home,
      roles: [Roles.Organizer, Roles.General],
    },
    {
      title: 'Tournaments',
      url: '/tournaments',
      icon: Trophy,
      roles: [Roles.Organizer],
      badge: 'New',
    },
    {
      title: 'Fixtures',
      url: '/fixtures',
      icon: Calendar,
      roles: [Roles.Organizer, Roles.General],
    },
    {
      title: 'Notifications',
      url: '/notifications',
      icon: Bell,
      roles: [Roles.Organizer, Roles.General],
    },
  ];

  quickActions: MenuItem[] = [
    {
      title: 'Create Tournament',
      url: '/tournaments/create',
      icon: PlusCircle,
      roles: [Roles.Organizer],
    }
  ];
  
  ngOnInit() {
    window.addEventListener('resize', this.handleResize);
  }

  ngOnDestroy() {
    window.removeEventListener('resize', this.handleResize);
  }

  handleResize = () => {
    this.internalOpen.set(window.innerWidth > 768);
  };
}
