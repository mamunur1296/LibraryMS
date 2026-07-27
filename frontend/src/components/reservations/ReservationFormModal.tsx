"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { createReservationSchema, CreateReservationFormData } from "@/lib/validations/reservation.schema";
import { reservationService } from "@/lib/services/reservation.service";
import { memberService } from "@/lib/services/member.service";
import { bookService } from "@/lib/services/book.service";
import { branchService } from "@/lib/services/branch.service";
import { useEffect, useState } from "react";

interface ReservationFormModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export function ReservationFormModal({ isOpen, onClose, onSuccess }: ReservationFormModalProps) {
  const [members, setMembers] = useState<any[]>([]);
  const [books, setBooks] = useState<any[]>([]);
  const [branches, setBranches] = useState<any[]>([]);
  const [loadingData, setLoadingData] = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<CreateReservationFormData>({
    resolver: zodResolver(createReservationSchema),
  });

  useEffect(() => {
    if (isOpen) {
      fetchInitialData();
      reset();
    }
  }, [isOpen, reset]);

  const fetchInitialData = async () => {
    setLoadingData(true);
    try {
      const [membersData, booksData, branchesData] = await Promise.all([
        memberService.search(undefined, "Active", 1, 100),
        bookService.search(undefined, undefined, undefined, undefined, 1, 100),
        branchService.getAll(false), 
      ]);
      setMembers(membersData.items || []);
      setBooks(booksData.items || []);
      setBranches(branchesData || []);
    } catch (error) {
      console.error("Failed to load form data", error);
    } finally {
      setLoadingData(false);
    }
  };

  if (!isOpen) return null;

  const onSubmit = async (data: CreateReservationFormData) => {
    try {
      await reservationService.create(data);
      reset();
      onSuccess();
      onClose();
    } catch (err: any) {
      alert(err.response?.data?.message || "Failed to place reservation.");
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm overflow-y-auto py-10">
      <div className="w-full max-w-lg bg-slate-900 border border-slate-700 rounded-2xl shadow-2xl overflow-hidden m-4">
        <div className="flex items-center justify-between px-6 py-4 border-b border-slate-800 bg-slate-900/50 sticky top-0 z-10">
          <h2 className="text-xl font-semibold text-white">
            Place Book Reservation
          </h2>
          <button onClick={onClose} className="text-slate-400 hover:text-white transition-colors">
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div className="p-6">
          {loadingData ? (
             <div className="flex justify-center py-8">
               <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-500"></div>
             </div>
          ) : (
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
              
              <div>
                <label className="block text-sm font-medium text-slate-300 mb-1">Select Member</label>
                <select
                  {...register("memberId")}
                  className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
                >
                  <option value="">-- Select Member --</option>
                  {members.map(m => (
                    <option key={m.id} value={m.id}>{m.fullName} ({m.membershipNumber})</option>
                  ))}
                </select>
                {errors.memberId && <p className="text-red-400 text-xs mt-1">{errors.memberId.message}</p>}
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-300 mb-1">Select Book</label>
                <select
                  {...register("bookId")}
                  className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
                >
                  <option value="">-- Select Book --</option>
                  {books.map(b => (
                    <option key={b.id} value={b.id}>
                      {b.title} (Available: {b.availableCopies} / Total: {b.totalCopies})
                    </option>
                  ))}
                </select>
                {errors.bookId && <p className="text-red-400 text-xs mt-1">{errors.bookId.message}</p>}
                <p className="text-xs text-slate-500 mt-2 italic">
                  Note: Members can reserve books even if they are currently available.
                </p>
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-300 mb-1">Pickup Branch</label>
                <select
                  {...register("branchId")}
                  className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
                >
                  <option value="">-- Branch --</option>
                  {branches.map(b => (
                    <option key={b.id} value={b.id}>{b.name}</option>
                  ))}
                </select>
                {errors.branchId && <p className="text-red-400 text-xs mt-1">{errors.branchId.message}</p>}
              </div>

              <div className="flex justify-end space-x-3 pt-4 border-t border-slate-800">
                <button
                  type="button"
                  onClick={onClose}
                  className="px-4 py-2 rounded-lg text-sm font-medium text-slate-300 hover:text-white hover:bg-slate-800 transition-colors"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={isSubmitting}
                  className="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-lg text-sm font-medium transition-colors shadow-lg shadow-indigo-500/20 disabled:opacity-50 flex items-center"
                >
                  {isSubmitting ? (
                    <span className="mr-2 border-2 border-white/20 border-t-white rounded-full w-4 h-4 animate-spin"></span>
                  ) : null}
                  Place Reservation
                </button>
              </div>
            </form>
          )}
        </div>
      </div>
    </div>
  );
}
