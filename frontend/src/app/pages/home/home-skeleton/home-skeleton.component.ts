import {Component, inject, OnInit} from '@angular/core';
import {HotToastService} from "@ngxpert/hot-toast";
import {NgClass} from "@angular/common";
import {Router, RouterOutlet} from "@angular/router";
import {WebsocketService} from "../../../services/websocket.service";
import {StateService} from "../../../services/state.service";
import {DeviceSidebarItemComponent} from "../../../components/device-sidebar-item/device-sidebar-item.component";
import {DashboardService} from "../../../services/dashboard.service";

@Component({
  selector: 'app-home-skeleton',
  standalone: true,
  imports: [
    NgClass,
    RouterOutlet,
    DeviceSidebarItemComponent
  ],
  templateUrl: './home-skeleton.component.html',
  styleUrl: './home-skeleton.component.css',
})
export class HomeSkeletonComponent implements OnInit {
  readonly ws = inject(WebsocketService);
  public hidden: boolean = false;
  stateService = inject(StateService);
  private readonly toast: HotToastService = inject(HotToastService);
  private readonly dashboardService = inject(DashboardService);
  readonly router = inject(Router);

  async ngOnInit(): Promise<void> {
    if (await this.dashboardService.checkStatus()){
      this.toast.success("Connected to server");
    } else {
      this.toast.error("Could not connect to server");
    }
  }

  logout() {
    this.toast.success("Logged out successfully");
  }

  async navigateHome() {
    await this.router.navigate(["/"]);
  }
}
