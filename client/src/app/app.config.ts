import {
  ApplicationConfig,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
  provideZonelessChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { errorInterceptor } from './core/interceptors/error-interceptor';
import { loadingInterceptor } from './core/interceptors/loading-interceptor';
import { InitService } from './core/services/init.service';
import { lastValueFrom } from 'rxjs';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZonelessChangeDetection(),
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([errorInterceptor, loadingInterceptor])),
    provideAppInitializer(() => {
      const initService = inject(InitService);

      return lastValueFrom(initService.init()).finally(() => {
        document.getElementById('initial-splash')?.remove();
      });
    }),
    // {
    //   provide: MAT_DIALOG_DEFAULT_OPTIONS,
    //   useValue: { autoFocus: 'dialog', restoreFocus: true },
    // },
  ],
};
