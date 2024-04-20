import {importProvidersFrom} from '@angular/core';
import {AppComponent} from './app/app.component';
import {AppRoutingModule} from './app/app-routing.module';
import {bootstrapApplication, BrowserModule} from '@angular/platform-browser';
import {provideHotToastConfig} from "@ngxpert/hot-toast";
import {provideHttpClient, withFetch} from "@angular/common/http";


bootstrapApplication(AppComponent, {
  providers: [importProvidersFrom(BrowserModule, AppRoutingModule), provideHttpClient(withFetch()),
    provideHotToastConfig(
      {
        duration: 5000,
        position: 'bottom-center',
        style: {
          backgroundColor: 'oklch(var(--b3))',
          color: 'oklch(var(--bc))',
        }
      }
    )
  ]
})
  .catch(err => console.error(err));
