import {Component, inject, OnInit} from '@angular/core';
import {DashboardService} from "../../../services/dashboard.service";
import {DeviceCardComponent} from "../../../components/device-card/device-card.component";
import {StateService} from "../../../services/state.service";

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    DeviceCardComponent
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  stateService = inject(StateService);
  dashboardService = inject(DashboardService);

  async ngOnInit(): Promise<void> {
    await this.dashboardService.getDevices();
  }


}
