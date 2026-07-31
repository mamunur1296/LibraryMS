import { apiClient } from '../api-client';
import { SettingDto, UpdateSettingRequest } from '../../types/settings.types';

export const settingsService = {
  getSettings: async (): Promise<SettingDto[]> => {
    const response = await apiClient.get<SettingDto[]>('/api/settings');
    return response.data;
  },

  updateSetting: async (key: string, data: UpdateSettingRequest): Promise<void> => {
    await apiClient.put(`/api/settings/${key}`, data);
  },
};
