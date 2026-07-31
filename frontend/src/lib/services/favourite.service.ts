import { BookDto } from '../../types/book.types';
import { apiClient } from '../api-client';

const FAVOURITES_KEY = 'libraryms_favourites';

export const favouriteService = {
  // Get from local storage
  getLocalFavourites: (): string[] => {
    try {
      const stored = localStorage.getItem(FAVOURITES_KEY);
      return stored ? JSON.parse(stored) : [];
    } catch {
      return [];
    }
  },

  // Save to local storage
  setLocalFavourites: (bookIds: string[]) => {
    localStorage.setItem(FAVOURITES_KEY, JSON.stringify(bookIds));
  },

  // Toggle local favourite
  toggleLocalFavourite: (bookId: string): boolean => {
    const favourites = favouriteService.getLocalFavourites();
    const isFavourite = favourites.includes(bookId);
    
    let newFavourites;
    if (isFavourite) {
      newFavourites = favourites.filter(id => id !== bookId);
    } else {
      newFavourites = [...favourites, bookId];
    }
    
    favouriteService.setLocalFavourites(newFavourites);
    return !isFavourite; // Returns true if added, false if removed
  },

  // Check if book is favourite (local)
  isLocalFavourite: (bookId: string): boolean => {
    return favouriteService.getLocalFavourites().includes(bookId);
  },

  // Get user favourites (API)
  getFavourites: async (): Promise<{ bookId: string; book: BookDto }[]> => {
    const response = await apiClient.get('/api/favourites');
    return response.data;
  },

  // Add favourite (API)
  addFavourite: async (bookId: string): Promise<void> => {
    await apiClient.post(`/api/favourites/${bookId}`);
  },

  // Remove favourite (API)
  removeFavourite: async (bookId: string): Promise<void> => {
    await apiClient.delete(`/api/favourites/${bookId}`);
  },
};
