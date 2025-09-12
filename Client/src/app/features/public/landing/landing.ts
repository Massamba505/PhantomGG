import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ThemeToggle } from '@/app/shared/components/ui/theme-toggle/theme-toggle';

@Component({
  selector: 'app-landing',
  imports: [RouterLink, ThemeToggle],
  standalone: true,
  templateUrl: './landing.html',
  styleUrl: './landing.css',
})
export class Landing {}
