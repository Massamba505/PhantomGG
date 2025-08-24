import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MainLayout } from './shared/components/layouts/main-layout/main-layout';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, MainLayout],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {}
