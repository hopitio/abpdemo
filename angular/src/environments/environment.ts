 import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:44335/',
  redirectUri: baseUrl,
  clientId: 'AbpAngular_App',
  responseType: 'code',
  scope: 'offline_access AbpAngular',
  requireHttps: true,
};

export const environment = {
  production: false,
  application: {
    baseUrl,
    name: 'AbpAngular',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://localhost:44335',
      rootNamespace: 'AbpAngular',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
} as Environment;
