import axios from 'axios';
import type { TinyUrlRecord } from '../types';

const API_BASE_URL = 'http://localhost:5226/api/tinyurls';

export const apiClient = {
  createTinyUrl: async (longUrl: string): Promise<TinyUrlRecord> => {
    try {
      const response = await axios.post<TinyUrlRecord>(`${API_BASE_URL}`, { longUrl });
      return response.data;
    } catch (error) {
      console.error('Error creating short URL:', error);
      throw error;
    }
  },

  getTinyUrl: async (id: string): Promise<TinyUrlRecord> => {
    try {
      const response = await axios.get<TinyUrlRecord>(`${API_BASE_URL}/${id}`);
      return response.data;
    } catch (error) {
      console.error('Error retrieving tiny URL:', error);
      throw error;
    }
  },

  listTinyUrls: async (): Promise<TinyUrlRecord[]> => {
    try {
      const response = await axios.get<TinyUrlRecord[]>(`${API_BASE_URL}`);
      return response.data;
    } catch (error) {
      console.error('Error fetching URLs:', error);
      throw error;
    }
  },

  deleteTinyUrl: async (id: string): Promise<TinyUrlRecord> => {
    try {
      const response = await axios.delete<TinyUrlRecord>(`${API_BASE_URL}/${id}`);
      return response.data;
    } catch (error) {
      console.error('Error deleting tiny URL:', error);
      throw error;
    }
  },
};
