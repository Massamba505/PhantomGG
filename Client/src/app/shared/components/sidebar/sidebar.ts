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
import { LucideAngularModule } from 'lucide-angular';
import { Roles } from '../../constants/roles';
import { LucideIcons } from '../ui/icons/lucide-icons';

interface MenuItem {
  title: string;
  url: string;
  icon: any;
  roles: Roles[];
  badge?: string;
}

@Component({
  selector: 'app-sidebar',
  imports: [CommonModule, RouterModule, LucideAngularModule],
  templateUrl: './sidebar.html',
})
export class Sidebar implements OnInit, OnDestroy {
  isOpen = input<boolean>(true);
  userRole = input.required<Roles>();
  toggle = output<void>();
  readonly icons = LucideIcons;

  private internalOpen = signal(true);
  
  actuallyOpen = computed(() => {
    if (typeof window !== 'undefined') {
      return window.innerWidth > 900 ? this.isOpen() : this.internalOpen();
    }
    return this.isOpen();
  });

  readonly MenuIcon = this.icons.MenuIcon;
  menuItems: MenuItem[] = [
    {
      title: 'Dashboard',
      url: '/organizer/dashboard',
      icon: this.icons.Home,
      roles: [Roles.Organizer],
    },
    {
      title: 'My Tournaments',
      url: '/organizer/tournaments',
      icon: this.icons.Trophy,
      roles: [Roles.Organizer],
    },
    {
      title: 'Fixtures',
      url: '/fixtures',
      icon: this.icons.Calendar,
      roles: [Roles.Organizer, Roles.User],
    },
    {
      title: 'Notifications',
      url: '/notifications',
      icon: this.icons.Bell,
      roles: [Roles.Organizer, Roles.User],
    },
  ];

  quickActions: MenuItem[] = [
    {
      title: 'Create Tournament',
      url: '/organizer/tournaments/create',
      icon: this.icons.CirclePlus,
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
    this.internalOpen.set(window.innerWidth > 900);
  };
}
