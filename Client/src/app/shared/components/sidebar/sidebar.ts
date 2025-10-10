import {
  Component,
  input,
  output,
  signal,
  OnInit,
  OnDestroy,
  effect,
} from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '../ui/icons/lucide-icons';
import { UserRoles } from '@/app/api/models';

interface MenuItem {
  title: string;
  url: string;
  icon: any;
  roles: UserRoles[];
  badge?: string;
}

@Component({
  selector: 'app-sidebar',
  imports: [CommonModule, RouterModule, LucideAngularModule],
  templateUrl: './sidebar.html',
})
export class Sidebar implements OnInit, OnDestroy {
  userRole = input.required<UserRoles>();
  toggle = output<void>();
  stateChange = output<boolean>();
  
  readonly icons = LucideIcons;

  isOpen = signal(window.innerWidth > 900);

  private stateEffect = effect(() => {
    this.stateChange.emit(this.isOpen());
  });

  readonly MenuIcon = this.icons.MenuIcon;
  menuItems: MenuItem[] = [
    {
      title: 'Dashboard',
      url: '/organizer/dashboard',
      icon: this.icons.Home,
      roles: [UserRoles.Organizer],
    },
    {
      title: 'Dashboard',
      url: '/user/dashboard',
      icon: this.icons.Home,
      roles: [UserRoles.User],
    },
    {
      title: 'My Tournaments',
      url: '/organizer/tournaments',
      icon: this.icons.Trophy,
      roles: [UserRoles.Organizer],
    },
    {
      title: 'My Teams',
      url: '/user/teams',
      icon: this.icons.Users,
      roles: [UserRoles.User],
    },
    {
      title: 'Tournaments',
      url: '/user/tournaments',
      icon: this.icons.Trophy,
      roles: [UserRoles.User],
    },
    {
      title: 'Fixtures',
      url: '/fixtures',
      icon: this.icons.Calendar,
      roles: [UserRoles.Organizer, UserRoles.User],
    },
    {
      title: 'Notifications',
      url: '/notifications',
      icon: this.icons.Bell,
      roles: [UserRoles.Organizer, UserRoles.User],
    },
  ];

  quickActions: MenuItem[] = [
    {
      title: 'Create Tournament',
      url: '/organizer/tournaments/create',
      icon: this.icons.CirclePlus,
      roles: [UserRoles.Organizer],
    },
    {
      title: 'Create Team',
      url: '/user/teams/create',
      icon: this.icons.UserPlus,
      roles: [UserRoles.User],
    }
  ];
  
  ngOnInit() {
    window.addEventListener('resize', this.handleResize);
  }

  ngOnDestroy() {
    window.removeEventListener('resize', this.handleResize);
  }

  handleResize = () => {
    this.isOpen.set(window.innerWidth > 900);
  };

  onToggle() {
    this.isOpen.update(prev => !prev);
    this.toggle.emit();
  }
}
