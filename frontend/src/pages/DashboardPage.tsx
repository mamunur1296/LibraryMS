import { useEffect, useState } from 'react';
import { dashboardService } from '@/lib/services/dashboard.service';
import { DashboardSummaryDto } from '@/types/dashboard.types';

export default function DashboardPage() {
  const [summary, setSummary] = useState<DashboardSummaryDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchDashboard = async () => {
      try {
        const data = await dashboardService.getSummary();
        setSummary(data);
      } catch (err: unknown) {
        // Show detailed error in dev for easier debugging
        const axiosErr = err as { response?: { status?: number; data?: { message?: string } }; message?: string };
        const status = axiosErr?.response?.status;
        const msg = axiosErr?.response?.data?.message || axiosErr?.message || 'Unknown error';
        setError(`Failed to load dashboard data. (${status ?? 'Network Error'}: ${msg})`);
      } finally {
        setLoading(false);
      }
    };
    void fetchDashboard();
  }, []);


  if (loading) {
    return (
      <div className="flex h-full items-center justify-center">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-500"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="p-4 bg-red-500/10 border border-red-500/50 rounded-xl text-red-400">
        {error}
      </div>
    );
  }

  const statCards = [
    { name: 'Total Books', value: summary?.totalBooks || 0, icon: '📚', color: 'from-blue-500 to-indigo-500' },
    { name: 'Total Members', value: summary?.totalMembers || 0, icon: '👥', color: 'from-emerald-500 to-teal-500' },
    { name: 'Active Borrows', value: summary?.activeBorrows || 0, icon: '🔄', color: 'from-amber-500 to-orange-500' },
    { name: 'Overdue Borrows', value: summary?.overdueBorrows || 0, icon: '⚠️', color: 'from-red-500 to-rose-500' },
    { name: 'Total Branches', value: summary?.totalBranches || 0, icon: '🏢', color: 'from-purple-500 to-fuchsia-500' },
    { name: 'Pending Reservations', value: summary?.pendingReservations || 0, icon: '⏳', color: 'from-pink-500 to-rose-500' },
  ];

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold tracking-tight text-white">Dashboard Overview</h1>
        <div className="flex items-center space-x-2">
          <span className="text-sm text-slate-400">Last updated: Just now</span>
        </div>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
        {statCards.map((stat) => (
          <div
            key={stat.name}
            className="relative overflow-hidden rounded-2xl bg-slate-900 border border-slate-800 p-6 shadow-sm transition-all hover:shadow-md hover:border-slate-700 group"
          >
            <div className="absolute top-0 right-0 p-4 opacity-10 text-6xl group-hover:scale-110 transition-transform duration-300">
              {stat.icon}
            </div>
            <div className="relative">
              <dt className="truncate text-sm font-medium text-slate-400">{stat.name}</dt>
              <dd className="mt-2 text-3xl font-semibold tracking-tight text-white">{stat.value}</dd>
            </div>
            <div className={`absolute bottom-0 left-0 h-1 w-full bg-gradient-to-r ${stat.color}`}></div>
          </div>
        ))}
      </div>

      {/* Financials / Fines */}
      <div className="grid grid-cols-1 gap-6 lg:grid-cols-2 mt-8">
        <div className="rounded-2xl bg-slate-900 border border-slate-800 p-6">
          <h3 className="text-lg font-medium text-white mb-4">Financial Overview</h3>
          <div className="space-y-4">
            <div className="flex justify-between items-center p-4 bg-slate-950 rounded-xl">
              <span className="text-slate-400">Total Fines Collected</span>
              <span className="text-xl font-semibold text-emerald-400">${summary?.totalLateFinesCollected?.toFixed(2) || '0.00'}</span>
            </div>
            <div className="flex justify-between items-center p-4 bg-slate-950 rounded-xl">
              <span className="text-slate-400">Pending Fines</span>
              <span className="text-xl font-semibold text-amber-400">${summary?.pendingLateFines?.toFixed(2) || '0.00'}</span>
            </div>
          </div>
        </div>

        <div className="rounded-2xl bg-slate-900 border border-slate-800 p-6 flex flex-col items-center justify-center text-center min-h-[200px]">
          <div className="w-16 h-16 rounded-full bg-slate-800 flex items-center justify-center mb-4">
            <span className="text-2xl">📊</span>
          </div>
          <h3 className="text-lg font-medium text-slate-300">Activity Chart</h3>
          <p className="text-sm text-slate-500 mt-1">Detailed analytics coming soon</p>
        </div>
      </div>
    </div>
  );
}
