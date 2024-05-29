import {Component, effect, inject, input, InputSignal, OnDestroy, signal, WritableSignal} from '@angular/core';
import {
  HistoricDataChartComponent
} from "../../charts/historic-data-chart/historic-data-chart.component";
import {BmeData} from "../../models/bme-data";
import {DataService} from "../../services/data.service";
import {NgClass} from "@angular/common";
import {WebsocketService} from "../../services/websocket.service";
import {ClientStopsRtc, ClientStartsRtc} from "../../services/events/client/client-starts-rtc";
import {StateService} from "../../services/state.service";


@Component({
  selector: 'app-historic-data',
  standalone: true,
  imports: [
    HistoricDataChartComponent,
    NgClass
  ],
  templateUrl: './historic-data.component.html',
  styleUrl: './historic-data.component.css'
})
export class HistoricDataComponent implements OnDestroy {
  mac: InputSignal<string> = input("", {alias: "mac"});
  bmeData: WritableSignal<BmeData[]> = signal<BmeData[]>([]);
  days: number = 7;
  private readonly dataService = inject(DataService);
  private readonly ws = inject(WebsocketService);
   stateService = inject(StateService);
  rtcOn: boolean = false;
  rtcData: WritableSignal<BmeData[]> = signal<BmeData[]>([]);

  constructor() {
    this.rtcData = this.stateService.rtcData.get(this.mac()) ?? signal<BmeData[]>([])
    effect(() => {
      if(this.days === 0) this.days = 7;
      this.getBmeData(this.days).then()
    });
  }

  async ngOnDestroy(): Promise<void> {
    this.stopRtc();
  }

  async getBmeData(number: number) {
    if(this.rtcOn) this.stopRtc();
    this.days = number;
    this.bmeData.set(await this.dataService.getDataPastXDays(this.mac(), number));
  }

  startRtc() {
    this.rtcOn = true;
    this.days = 0;
    this.ws.sendJson(new ClientStartsRtc({mac: this.mac()}));
  }

  stopRtc() {
    this.rtcOn = false;
    this.days = 7;
    this.ws.sendJson(new ClientStopsRtc({mac: this.mac()}));
  }

  getRtcBmeDataSignal() {
    let sig = this.stateService.rtcData.get(this.mac());
    if (!sig){
      sig = signal<BmeData[]>([]);
      this.stateService.rtcData.set(this.mac(), sig);
    }
    return sig;
  }
}
