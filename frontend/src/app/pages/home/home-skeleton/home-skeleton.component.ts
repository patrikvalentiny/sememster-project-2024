import {Component, inject, OnInit} from '@angular/core';
import {HotToastService} from "@ngxpert/hot-toast";
import {NgClass} from "@angular/common";
import {RouterOutlet} from "@angular/router";
import {HttpClient, HttpResponse} from "@angular/common/http";
import {environment} from "../../../../environments/environment";
import {firstValueFrom} from "rxjs";
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
  styleUrl: './home-skeleton.component.css'
})
export class HomeSkeletonComponent implements OnInit {
  readonly ws = inject(WebsocketService);
  public hidden: boolean = false;
  private readonly toast: HotToastService = inject(HotToastService);
  private readonly http: HttpClient = inject(HttpClient);
  private readonly dashboardService = inject(DashboardService);

  stateService = inject(StateService);

  async ngOnInit(): Promise<void> {
    await this.checkStatus();
  }

  logout() {
    this.toast.success("Logged out successfully");
  }

  private async checkStatus() {
    try {
      const call = this.http.get<string>(environment.restBaseUrl + `/status`, {
        observe: "response",
        responseType: "text" as "json"
      })
      const response = await firstValueFrom<HttpResponse<string>>(call);
      if (response.status === 200) {
        this.toast.success("Server is up");
      }
    } catch (e) {
      this.toast.error("Server is down");
    }

  }
}
