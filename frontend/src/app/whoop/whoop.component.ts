import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WhoopService, WhoopDashboard } from '../services/whoop.service';

@Component({
  selector: 'app-whoop',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './whoop.component.html',
  styleUrl: './whoop.component.css'
})
export class WhoopComponent implements OnInit {
  connected = false;
  dashboard: WhoopDashboard | null = null;
  loading = false;

  constructor(private whoopService: WhoopService) {}

  ngOnInit(): void {
    if (window.location.search.includes('whoop=connected')) {
      this.connected = true;
      this.loadDashboard();
      return;
    }
    this.whoopService.getStatus().subscribe({
      next: (res) => {
        this.connected = res.connected;
        if (this.connected) this.loadDashboard();
      }
    });
  }

  connect(): void {
    this.whoopService.getAuthorizeUrl().subscribe({
      next: (res) => window.location.href = res.url
    });
  }

  loadDashboard(): void {
    this.loading = true;
    this.whoopService.getDashboard().subscribe({
      next: (data) => { this.dashboard = data; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }
}
