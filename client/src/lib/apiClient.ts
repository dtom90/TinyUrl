import axios, { AxiosError } from 'axios';
import type { TinyUrlRecord, TinyUrlRequest } from '../types';
import { config } from '../config';

const TINYURLS_API_BASE_URL = `${config.apiBaseUrl}/api/tinyurls`;

export const queryKeys = {
  tinyUrls: 'tinyUrls',
  tinyUrl: (id: string) => ['tinyUrl', id],
} as const;

export const apiClient = {
  createTinyUrl: async (request: TinyUrlRequest): Promise<TinyUrlRecord> => {
    try {
      const response = await axios.post<TinyUrlRecord>(`${TINYURLS_API_BASE_URL}`, request);
      return response.data;
    } catch (error) {
      console.error(error);
      throw new Error(getErrorMessage(error, 'create'));
    }
  },

  getTinyUrl: async (id: string): Promise<TinyUrlRecord> => {
    try {
      const response = await axios.get<TinyUrlRecord>(`${TINYURLS_API_BASE_URL}/${id}`);
      return response.data;
    } catch (error) {
      console.error(error);
      throw new Error(getErrorMessage(error, 'get'));
    }
  },

  listTinyUrls: async (): Promise<TinyUrlRecord[]> => {
    try {
      const response = await axios.get<TinyUrlRecord[]>(`${TINYURLS_API_BASE_URL}`);
      return response.data;
    } catch (error) {
      console.error(error);
      throw new Error(getErrorMessage(error, 'list'));
    }
  },

  deleteTinyUrl: async (id: string): Promise<TinyUrlRecord> => {
    try {
      const response = await axios.delete<TinyUrlRecord>(`${TINYURLS_API_BASE_URL}/${id}`);
      return response.data;
    } catch (error) {
      console.error(error);
      throw new Error(getErrorMessage(error, 'delete'));
    }
  },
};

export const getErrorMessage = (error: unknown, action: string): string => {
  let errorMessage = `Failed to ${action} short URL. Please try again.`;
  if (error instanceof AxiosError) {
    errorMessage = error.response?.data?.error || error.message;
  } else if (error instanceof Error) {
    errorMessage = error.message;
  }
  return errorMessage;
};
