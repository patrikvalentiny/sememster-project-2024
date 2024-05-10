import {Component, inject, Input, OnInit} from '@angular/core';
import {WebsocketService} from "../../services/websocket.service";
import {ClientControlsMotor} from "../../services/events/client/client-controls-motor";
import {StateService} from "../../services/state.service";
import {NgClass} from "@angular/common";
import {MotorService} from "../../services/motor.service";

@Component({
  selector: 'app-motor-control',
  standalone: true,
  imports: [
    NgClass
  ],
  templateUrl: './motor-control.component.html',
  styleUrl: './motor-control.component.css'
})
export class MotorControlComponent implements OnInit{
  ws = inject(WebsocketService);
  state = inject(StateService);
  motorService = inject(MotorService);

  @Input() mac?: string
  value: number = 0;

  async ngOnInit(): Promise<void> {
    this.value = await this.motorService.getMotorPosition(this.mac!);
  }

  async move(val: any) {
    this.value = val.value;
    this.state.motorMoving.set(this.mac!, true);
    this.ws.sendJson(new ClientControlsMotor({position: this.value, mac:this.mac}));
  }
}
