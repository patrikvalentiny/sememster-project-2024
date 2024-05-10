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
  editingMaxPosition: boolean = false;
  max:number = 500
  step:number = 20

  @Input() mac?: string
  value: number = 0;

  async ngOnInit(): Promise<void> {
    const response =  await this.motorService.getMotorPosition(this.mac!);
    this.value = response.lastMotorPosition;
    this.max = response.maxMotorPosition;
  }

  async move(val: any) {
    this.value = val.value;
    this.state.motorMoving.set(this.mac!, true);
    this.ws.sendJson(new ClientControlsMotor({position: this.value, mac:this.mac}));
  }

  async editMaxPosition() {
    this.editingMaxPosition = !this.editingMaxPosition;
    if (this.editingMaxPosition) {
      this.value = this.state.motorPosition.get(this.mac!)!;
    } else {
      this.max = await this.motorService.setMaxPosition(this.mac!, this.value);
    }
  }

  reduceMax() {
    this.max -= this.step;
    this.value = this.max;
    this.state.motorMoving.set(this.mac!, true);
    this.ws.sendJson(new ClientControlsMotor({position: this.value, mac:this.mac}));
  }
   increaseMax(){
    this.max += this.step;
    this.value = this.max;
    this.state.motorMoving.set(this.mac!, true);
    this.ws.sendJson(new ClientControlsMotor({position: this.value, mac:this.mac}));
  }
}
