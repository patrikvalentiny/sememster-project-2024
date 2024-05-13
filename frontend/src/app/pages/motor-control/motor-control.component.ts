import {Component, inject, Input, OnInit} from '@angular/core';
import {WebsocketService} from "../../services/websocket.service";
import {ClientControlsMotor} from "../../services/events/client/client-controls-motor";
import {StateService} from "../../services/state.service";
import {NgClass} from "@angular/common";
import {MotorService} from "../../services/motor.service";
import {FormControl, ReactiveFormsModule} from "@angular/forms";
import {ClientStartsListeningToMotor} from "../../services/events/client/client-starts-listening-to-motor";


@Component({
  selector: 'app-motor-control',
  standalone: true,
  imports: [
    NgClass,
    ReactiveFormsModule
  ],
  templateUrl: './motor-control.component.html',
  styleUrl: './motor-control.component.css'
})
export class MotorControlComponent implements OnInit{
  @Input() mac?: string

  ws = inject(WebsocketService);
  state = inject(StateService);
  motorService = inject(MotorService);

  max:number = 500
  step:number = 20
  value: number = 0;
  directionToggle = new FormControl(false);

  async ngOnInit(): Promise<void> {
    const response =  await this.motorService.getMotorPosition(this.mac!);
    this.value = response.lastMotorPosition;
    this.max = response.maxMotorPosition;
    this.directionToggle.setValue(response.motorReversed);
    this.ws.sendJson(new ClientStartsListeningToMotor({mac:this.mac}));

  }

  async move(val: any) {
    this.value = val.value;
    this.state.motorMoving.set(this.mac!, true);
    this.ws.sendJson(new ClientControlsMotor({position: this.value, mac:this.mac}));
  }

  async changeDirection() {
    await this.motorService.setMotorDirection(this.mac!, this.directionToggle.value!);
  }

  async setMaxToCurrent() {
    this.max = await this.motorService.setMaxPosition(this.mac!, this.value);
  }
  async increaseMax(){
    this.max += this.step;
    this.value = this.max;
    this.state.motorMoving.set(this.mac!, true);
    await this.motorService.setMaxPosition(this.mac!, this.max);
    this.ws.sendJson(new ClientControlsMotor({position: this.value, mac:this.mac}));
  }
}
