import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZonelessChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient } from '@angular/common/http';
import {MAT_DIALOG_DEFAULT_OPTIONS } from '@angular/material/dialog';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZonelessChangeDetection(),
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(),
    {
      provide: MAT_DIALOG_DEFAULT_OPTIONS,
      useValue: { autoFocus: 'dialog', restoreFocus: true },
    },
  ],
};
