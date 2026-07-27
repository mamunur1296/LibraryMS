import { apiClient } from "../api-client";
import { ReservationDto, CreateReservationRequest, ReservationQueueDto, PagedResult } from "../../types/reservation.types";

export const reservationService = {
  async search(
    memberId?: string, 
    bookId?: string, 
    status?: string, 
    page: number = 1, 
    pageSize: number = 10
  ): Promise<PagedResult<ReservationDto>> {
    const params = new URLSearchParams();
    if (memberId) params.append("memberId", memberId);
    if (bookId) params.append("bookId", bookId);
    if (status) params.append("status", status);
    params.append("page", page.toString());
    params.append("pageSize", pageSize.toString());

    const response = await apiClient.get<PagedResult<ReservationDto>>(`/api/Reservations?${params.toString()}`);
    return response.data;
  },

  async create(data: CreateReservationRequest): Promise<ReservationDto> {
    const response = await apiClient.post<ReservationDto>("/api/Reservations", data);
    return response.data;
  },

  async cancel(id: string): Promise<void> {
    await apiClient.post(`/api/Reservations/${id}/cancel`);
  },

  async getQueue(bookId: string, branchId: string): Promise<ReservationQueueDto> {
    const response = await apiClient.get<ReservationQueueDto>(`/api/Reservations/queue?bookId=${bookId}&branchId=${branchId}`);
    return response.data;
  }
};
