import { apiClient } from "@/lib/api-client";
import {
  MemberDto, PagedResult, CreateMemberRequest,
  UpdateMemberRequest, SuspendMemberRequest,
  MemberProfileStatsDto, ResetMemberPasswordRequest,
  CreateMemberUserRequest, RenewMembershipRequest
} from "@/types/member.types";

export const memberService = {
  async search(
    searchTerm?: string, 
    status?: string, 
    page: number = 1, 
    pageSize: number = 10
  ): Promise<PagedResult<MemberDto>> {
    const params = new URLSearchParams();
    if (searchTerm) params.append("searchTerm", searchTerm);
    if (status) params.append("status", status);
    params.append("page", page.toString());
    params.append("pageSize", pageSize.toString());

    const response = await apiClient.get<PagedResult<MemberDto>>(`/api/Members?${params.toString()}`);
    return response.data;
  },

  async getById(id: string): Promise<MemberDto> {
    const response = await apiClient.get<MemberDto>(`/api/Members/${id}`);
    return response.data;
  },

  async create(data: CreateMemberRequest): Promise<MemberDto> {
    const response = await apiClient.post<MemberDto>("/api/Members", data);
    return response.data;
  },

  async update(id: string, data: Omit<UpdateMemberRequest, "id">): Promise<MemberDto> {
    const response = await apiClient.put<MemberDto>(`/api/Members/${id}`, { id, ...data });
    return response.data;
  },

  async delete(id: string): Promise<void> {
    await apiClient.delete(`/api/Members/${id}`);
  },

  async suspend(id: string, data: Omit<SuspendMemberRequest, "id">): Promise<MemberDto> {
    const response = await apiClient.post<MemberDto>(`/api/Members/${id}/suspend`, { id, ...data });
    return response.data;
  },

  async activate(id: string): Promise<MemberDto> {
    const response = await apiClient.post<MemberDto>(`/api/Members/${id}/activate`);
    return response.data;
  },

  async getStats(id: string): Promise<MemberProfileStatsDto> {
    const response = await apiClient.get<MemberProfileStatsDto>(`/api/Members/${id}/stats`);
    return response.data;
  },

  async resetPassword(id: string, data: ResetMemberPasswordRequest): Promise<void> {
    await apiClient.post(`/api/Members/${id}/reset-password`, data);
  },

  async createAccount(id: string, data: CreateMemberUserRequest): Promise<void> {
    await apiClient.post(`/api/Members/${id}/create-account`, data);
  },

  async renew(id: string, data: RenewMembershipRequest): Promise<MemberDto> {
    const response = await apiClient.post<MemberDto>(`/api/Members/${id}/renew`, data);
    return response.data;
  },
};
