import { apiClient } from "../api-client";
import { BranchDto, CreateBranchRequest, UpdateBranchRequest } from "../../types/branch.types";

export const branchService = {
  async getAll(includeInactive: boolean = false): Promise<BranchDto[]> {
    const response = await apiClient.get<BranchDto[]>(`/api/Branches?includeInactive=${includeInactive}`);
    return response.data;
  },

  async getById(id: string): Promise<BranchDto> {
    const response = await apiClient.get<BranchDto>(`/api/Branches/${id}`);
    return response.data;
  },

  async create(data: CreateBranchRequest): Promise<BranchDto> {
    const response = await apiClient.post<BranchDto>("/api/Branches", data);
    return response.data;
  },

  async update(id: string, data: Omit<UpdateBranchRequest, "id">): Promise<BranchDto> {
    const response = await apiClient.put<BranchDto>(`/api/Branches/${id}`, { id, ...data });
    return response.data;
  },

  async activate(id: string): Promise<BranchDto> {
    const response = await apiClient.post<BranchDto>(`/api/Branches/${id}/activate`);
    return response.data;
  },

  async deactivate(id: string): Promise<BranchDto> {
    const response = await apiClient.post<BranchDto>(`/api/Branches/${id}/deactivate`);
    return response.data;
  }
};
