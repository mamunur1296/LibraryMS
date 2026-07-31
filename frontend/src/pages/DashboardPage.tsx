import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { dashboardService } from '@/lib/services/dashboard.service';
import { memberService } from '@/lib/services/member.service';
<<<<<<< Updated upstream
import { DashboardSummaryDto, AdminDashboardSummaryDto, BranchDashboardSummaryDto } from '@/types/dashboard.types';
import { MemberProfileStatsDto } from '@/types/member.types';
=======
import { favouriteService } from '@/lib/services/favourite.service';
import { DashboardSummaryDto, AdminDashboardSummaryDto, BranchDashboardSummaryDto } from '@/types/dashboard.types';
import { MemberProfileStatsDto } from '@/types/member.types';
import { BookDto } from '@/types/book.types';
>>>>>>> Stashed changes
import { useAuth } from '@/contexts/AuthContext';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip as RechartsTooltip, Legend, ResponsiveContainer } from 'recharts';

export default function DashboardPage() {
  const { user } = useAuth();
  const [summary, setSummary] = useState<DashboardSummaryDto | null>(null);
  const [adminSummary, setAdminSummary] = useState<AdminDashboardSummaryDto | null>(null);
  const [memberStats, setMemberStats] = useState<MemberProfileStatsDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  
<<<<<<< Updated upstream
=======
  // Favourites state
  const [showFavourites, setShowFavourites] = useState(false);
  const [favourites, setFavourites] = useState<BookDto[]>([]);
  const [loadingFavourites, setLoadingFavourites] = useState(false);
  
>>>>>>> Stashed changes
  // Admin Branch Filter
  const [selectedBranchId, setSelectedBranchId] = useState<string>('all');

  useEffect(() => {
    const fetchDashboard = async () => {
      try {
        if (user?.role === 'Member' && user?.memberId) {
          const stats = await memberService.getStats(user.memberId);
          setMemberStats(stats);
        } else if (user?.role === 'Admin') {
          const data = await dashboardService.getAdminSummary();
          setAdminSummary(data);
          setSummary(data.totalSummary); // For top cards
        } else {
          const data = await dashboardService.getSummary();
          setSummary(data);
        }
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
    if (user) {
      void fetchDashboard();
    }
  }, [user]);


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

<<<<<<< Updated upstream
=======
  const toggleFavourites = async () => {
    if (!showFavourites) {
      setLoadingFavourites(true);
      try {
        const data = await favouriteService.getFavourites();
        setFavourites(data.map((item: any) => item.book));
      } catch (err) {
        console.error('Failed to load favourites', err);
      } finally {
        setLoadingFavourites(false);
      }
    }
    setShowFavourites(!showFavourites);
  };

>>>>>>> Stashed changes
  const adminCards = [
    { name: 'Total Books', value: summary?.totalBooks || 0, icon: '📚', color: 'from-blue-500 to-indigo-500' },
    { name: 'Total Members', value: summary?.totalMembers || 0, icon: '👥', color: 'from-emerald-500 to-teal-500' },
    { name: 'Active Borrows', value: summary?.activeBorrows || 0, icon: '🔄', color: 'from-amber-500 to-orange-500' },
    { name: 'Overdue Borrows', value: summary?.overdueBorrows || 0, icon: '⚠️', color: 'from-red-500 to-rose-500' },
    { name: 'Total Branches', value: summary?.totalBranches || 0, icon: '🏢', color: 'from-purple-500 to-fuchsia-500' },
    { name: 'Pending Reservations', value: summary?.pendingReservations || 0, icon: '⏳', color: 'from-pink-500 to-rose-500' },
  ];

  const memberCards = [
<<<<<<< Updated upstream
    { name: 'Active Borrows', value: memberStats?.activeBorrows || 0, icon: '🔄', color: 'from-blue-500 to-indigo-500' },
    { name: 'Total Borrows', value: memberStats?.totalBorrows || 0, icon: '📚', color: 'from-emerald-500 to-teal-500' },
    { name: 'Overdue Borrows', value: memberStats?.overdueBorrows || 0, icon: '⚠️', color: 'from-red-500 to-rose-500' },
    { name: 'Pending Reservations', value: memberStats?.activeReservations || 0, icon: '⏳', color: 'from-purple-500 to-fuchsia-500' },
=======
    { 
      name: 'Active Borrows', 
      value: memberStats?.activeBorrows || 0, 
      icon: '📚', 
      color: 'from-blue-500 to-indigo-500',
      extra: memberStats?.nearestDueDate ? <span className="text-xs text-slate-400 bg-slate-800 px-2 py-1 rounded-full mt-2 inline-block">Due: {new Date(memberStats.nearestDueDate).toLocaleDateString()}</span> : null
    },
    { 
      name: 'Total Borrowed', 
      value: memberStats?.totalBorrows || 0, 
      icon: '📖', 
      color: 'from-emerald-500 to-teal-500' 
    },
    { 
      name: 'Overdue', 
      value: memberStats?.overdueBorrows || 0, 
      icon: '⚠️', 
      color: 'from-red-500 to-rose-500',
      extra: (memberStats?.overdueBorrows ?? 0) > 0 ? <span className="text-xs text-red-400 bg-red-900/30 px-2 py-1 rounded-full mt-2 inline-block">Fine accumulating daily</span> : null
    },
    { 
      name: 'Pending Reservations', 
      value: memberStats?.activeReservations || 0, 
      icon: '⏳', 
      color: 'from-purple-500 to-fuchsia-500',
      extra: (memberStats?.activeReservations ?? 0) > 0 ? <span className="text-xs text-slate-400 bg-slate-800 px-2 py-1 rounded-full mt-2 inline-block">Queue-এ আছে</span> : null
    },
    {
      name: 'Outstanding Fine',
      value: `$${(memberStats?.totalFinesDue || 0).toFixed(2)}`,
      icon: '💰',
      color: 'from-amber-500 to-orange-500',
      extra: (memberStats?.totalFinesDue ?? 0) > 0 ? <span className="text-xs text-red-400 bg-red-900/30 px-2 py-1 rounded-full mt-2 inline-block">Red alert</span> : null
    },
    {
      name: 'Total Fine Paid',
      value: `$${(memberStats?.totalFinesPaid || 0).toFixed(2)}`,
      icon: '✅',
      color: 'from-emerald-500 to-green-500'
    },
    {
      name: 'Membership Expires',
      value: memberStats?.membershipExpiry ? new Date(memberStats.membershipExpiry).toLocaleDateString() : 'N/A',
      icon: '📅',
      color: 'from-cyan-500 to-blue-500',
      extra: memberStats?.membershipExpiry && new Date(memberStats.membershipExpiry).getTime() - new Date().getTime() < 30 * 24 * 60 * 60 * 1000 ? <button className="text-xs text-white bg-indigo-600 hover:bg-indigo-500 px-2 py-1 rounded mt-2 inline-block transition-colors">Renew</button> : null
    },
    {
      name: 'Favourites Count',
      value: memberStats?.favouriteCount || 0,
      icon: '❤️',
      color: 'from-pink-500 to-rose-500',
      extra: <button onClick={toggleFavourites} className="text-xs text-pink-400 hover:text-pink-300 mt-2 inline-block underline cursor-pointer">{showFavourites ? 'Hide list' : 'Show list'}</button>
    }
>>>>>>> Stashed changes
  ];

  const statCards = user?.role === 'Member' ? memberCards : adminCards;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold tracking-tight text-white">
            {user?.role === 'Librarian' ? 'Branch Dashboard' : 'Dashboard Overview'}
          </h1>
          {user?.role === 'Librarian' && (
            <p className="text-sm text-slate-400 mt-1">Welcome back, {user.username} (Librarian)</p>
          )}
        </div>
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
<<<<<<< Updated upstream
=======
              {'extra' in stat && stat.extra && (
                <div className="mt-1">
                  {stat.extra}
                </div>
              )}
>>>>>>> Stashed changes
            </div>
            <div className={`absolute bottom-0 left-0 h-1 w-full bg-gradient-to-r ${stat.color}`}></div>
          </div>
        ))}
      </div>

      {/* Additional sections depending on role */}
      {user?.role === 'Member' && (
<<<<<<< Updated upstream
        <div className="grid grid-cols-1 gap-6 lg:grid-cols-2 mt-8">
          <div className="rounded-2xl bg-slate-900 border border-slate-800 p-6">
=======
        <>
          {showFavourites && (
            <div className="mt-8 rounded-2xl bg-slate-900 border border-slate-800 overflow-hidden">
              <div className="p-6 border-b border-slate-800 flex justify-between items-center">
                <h3 className="text-lg font-medium text-white flex items-center gap-2">
                  <span className="text-rose-500">❤️</span> My Favourites
                </h3>
              </div>
              <div className="overflow-x-auto">
                {loadingFavourites ? (
                  <div className="p-8 text-center text-slate-400">Loading favourites...</div>
                ) : favourites.length > 0 ? (
                  <table className="w-full text-left text-sm text-slate-300">
                    <thead className="text-xs uppercase bg-slate-900/50 border-b border-slate-800 text-slate-400">
                      <tr>
                        <th className="px-6 py-4 font-medium">Book Title</th>
                        <th className="px-6 py-4 font-medium">Author</th>
                        <th className="px-6 py-4 font-medium text-center">ISBN</th>
                        <th className="px-6 py-4 font-medium text-right">Available Copies</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-800/50">
                      {favourites.map((book) => (
                        <tr key={book.id} className="hover:bg-slate-800/20 transition-colors">
                          <td className="px-6 py-4 font-medium text-white">{book.title}</td>
                          <td className="px-6 py-4">{book.authorName}</td>
                          <td className="px-6 py-4 text-center">{book.isbn}</td>
                          <td className="px-6 py-4 text-right">
                            {book.availableCopies > 0 ? (
                              <span className="text-emerald-400">{book.availableCopies} available</span>
                            ) : (
                              <span className="text-rose-400">Out of stock</span>
                            )}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                ) : (
                  <div className="p-8 text-center text-slate-400">No favourites added yet.</div>
                )}
              </div>
            </div>
          )}
          <div className="grid grid-cols-1 gap-6 lg:grid-cols-2 mt-8">
            <div className="rounded-2xl bg-slate-900 border border-slate-800 p-6">
>>>>>>> Stashed changes
            <h3 className="text-lg font-medium text-white mb-4">Financial Overview</h3>
            <div className="flex justify-between items-center p-4 bg-slate-950 rounded-xl">
              <span className="text-slate-400">Total Fines Due</span>
              <span className="text-xl font-semibold text-amber-400">${memberStats?.totalFinesDue?.toFixed(2) || '0.00'}</span>
            </div>
          </div>
          <div className="rounded-2xl bg-slate-900 border border-slate-800 p-6">
            <h3 className="text-lg font-medium text-white mb-4">Quick Actions</h3>
            <div className="space-y-3">
              <Link to="/catalog" className="block w-full text-center py-2 bg-indigo-600 hover:bg-indigo-500 rounded-lg text-white font-medium transition-colors">
                Browse Catalog
              </Link>
              <Link to="/borrows" className="block w-full text-center py-2 bg-slate-800 hover:bg-slate-700 rounded-lg text-white font-medium transition-colors">
                View My Borrows
              </Link>
            </div>
          </div>
        </div>
<<<<<<< Updated upstream
=======
      </>
>>>>>>> Stashed changes
      )}

      {user?.role === 'Librarian' && (
        <div className="grid grid-cols-1 gap-6 lg:grid-cols-3 mt-8">
          <div className="lg:col-span-2 rounded-2xl bg-slate-900 border border-slate-800 p-6 flex flex-col">
            <h3 className="text-lg font-medium text-white mb-4">Today's Activity Feed</h3>
            <div className="flex-1 flex items-center justify-center min-h-[200px] border-2 border-dashed border-slate-800 rounded-xl bg-slate-950/50 text-slate-500">
              Activity feed stream will appear here...
            </div>
          </div>
          <div className="rounded-2xl bg-slate-900 border border-slate-800 p-6">
            <h3 className="text-lg font-medium text-white mb-4">Quick Actions</h3>
            <div className="space-y-3">
              <Link to="/borrows?action=new" className="block w-full text-center py-3 bg-indigo-600 hover:bg-indigo-500 rounded-lg text-white font-medium transition-colors">
                + New Borrow
              </Link>
              <Link to="/borrows?action=return" className="block w-full text-center py-3 bg-emerald-600/20 hover:bg-emerald-600/30 text-emerald-400 border border-emerald-600/50 rounded-lg font-medium transition-colors">
                Process Return
              </Link>
              <Link to="/members" className="block w-full text-center py-3 bg-slate-800 hover:bg-slate-700 rounded-lg text-white font-medium transition-colors border border-slate-700">
                + New Member
              </Link>
            </div>
          </div>
        </div>
      )}

      {user?.role === 'Admin' && (
        <div className="mt-8 space-y-6">
          <div className="flex justify-between items-center bg-slate-900 border border-slate-800 p-4 rounded-2xl">
            <h3 className="text-lg font-medium text-white">System Dashboard</h3>
            <select
              value={selectedBranchId}
              onChange={(e) => setSelectedBranchId(e.target.value)}
              className="px-4 py-2 bg-slate-950 border border-slate-700 rounded-xl text-slate-300 focus:outline-none focus:ring-2 focus:ring-indigo-500"
            >
              <option value="all">All Branches</option>
              {adminSummary?.branchSummaries.map((b) => (
                <option key={b.branchId} value={b.branchId}>{b.branchName}</option>
              ))}
            </select>
          </div>

          <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
            <div className="rounded-2xl bg-slate-900 border border-slate-800 p-6">
              <h3 className="text-lg font-medium text-white mb-4">System Financial Overview</h3>
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
            
            <div className="rounded-2xl bg-slate-900 border border-slate-800 p-6 flex flex-col min-h-[300px]">
              <h3 className="text-lg font-medium text-slate-300 mb-6">Activity by Branch</h3>
              <div className="flex-1 w-full h-[250px]">
                {adminSummary?.branchSummaries && adminSummary.branchSummaries.length > 0 ? (
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart
                      data={adminSummary.branchSummaries}
                      margin={{ top: 10, right: 10, left: -20, bottom: 0 }}
                    >
                      <CartesianGrid strokeDasharray="3 3" stroke="#334155" vertical={false} />
                      <XAxis dataKey="branchName" stroke="#94a3b8" fontSize={12} tickLine={false} axisLine={false} />
                      <YAxis stroke="#94a3b8" fontSize={12} tickLine={false} axisLine={false} />
                      <RechartsTooltip 
                        contentStyle={{ backgroundColor: '#0f172a', borderColor: '#1e293b', color: '#f8fafc', borderRadius: '0.5rem' }}
                        itemStyle={{ color: '#e2e8f0' }}
                      />
                      <Legend wrapperStyle={{ paddingTop: '10px' }} />
                      <Bar dataKey="activeBorrows" name="Active Borrows" fill="#6366f1" radius={[4, 4, 0, 0]} />
                      <Bar dataKey="overdueBorrows" name="Overdue Borrows" fill="#f43f5e" radius={[4, 4, 0, 0]} />
                    </BarChart>
                  </ResponsiveContainer>
                ) : (
                  <div className="flex items-center justify-center h-full text-slate-500">No data available</div>
                )}
              </div>
            </div>
          </div>

          <div className="rounded-2xl bg-slate-900 border border-slate-800 overflow-hidden">
            <div className="p-6 border-b border-slate-800 flex justify-between items-center">
              <h3 className="text-lg font-medium text-white">Per-Branch Comparison</h3>
            </div>
            <div className="overflow-x-auto">
              <table className="w-full text-left text-sm text-slate-300">
                <thead className="text-xs uppercase bg-slate-900/50 border-b border-slate-800 text-slate-400">
                  <tr>
                    <th className="px-6 py-4 font-medium">Branch</th>
                    <th className="px-6 py-4 font-medium text-center">Total Books</th>
                    <th className="px-6 py-4 font-medium text-right">Active Borrows</th>
                    <th className="px-6 py-4 font-medium text-right">Overdue</th>
                    <th className="px-6 py-4 font-medium text-right">Revenue</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-800/50">
                  {(selectedBranchId === 'all' ? adminSummary?.branchSummaries : adminSummary?.branchSummaries.filter(b => b.branchId === selectedBranchId))?.map((branch: BranchDashboardSummaryDto) => (
                    <tr key={branch.branchId} className="hover:bg-slate-800/20 transition-colors">
                      <td className="px-6 py-4 font-medium text-white">{branch.branchName}</td>
                      <td className="px-6 py-4 text-center">{branch.totalBooks}</td>
                      <td className="px-6 py-4 text-right">{branch.activeBorrows}</td>
                      <td className="px-6 py-4 text-right text-rose-400">{branch.overdueBorrows}</td>
                      <td className="px-6 py-4 text-right text-emerald-400">${branch.totalRevenue.toFixed(2)}</td>
                    </tr>
                  ))}
                  {/* Total Row */}
                  {selectedBranchId === 'all' && (
                    <tr className="bg-slate-950 font-semibold border-t-2 border-slate-700">
                      <td className="px-6 py-4 text-white">Total All Branches</td>
                      <td className="px-6 py-4 text-center text-slate-300">
                        {adminSummary?.branchSummaries.reduce((acc: number, curr: BranchDashboardSummaryDto) => acc + curr.totalBooks, 0)}
                      </td>
                      <td className="px-6 py-4 text-right text-indigo-300">
                        {adminSummary?.branchSummaries.reduce((acc: number, curr: BranchDashboardSummaryDto) => acc + curr.activeBorrows, 0)}
                      </td>
                      <td className="px-6 py-4 text-right text-rose-400">
                        {adminSummary?.branchSummaries.reduce((acc: number, curr: BranchDashboardSummaryDto) => acc + curr.overdueBorrows, 0)}
                      </td>
                      <td className="px-6 py-4 text-right text-emerald-400">
                        ${adminSummary?.branchSummaries.reduce((acc: number, curr: BranchDashboardSummaryDto) => acc + curr.totalRevenue, 0).toFixed(2)}
                      </td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
