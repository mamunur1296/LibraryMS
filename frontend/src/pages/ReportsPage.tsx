import { useEffect, useState } from 'react';
import { reportService } from '@/lib/services/report.service';
import { ReportFilterBar } from '@/components/reports/ReportFilterBar';
import { PagedResult, OverdueReportDto, PopularBookDto, MemberActivityDto } from '@/types/report.types';

type TabType = 'overdue' | 'popular' | 'activity';

export default function ReportsPage() {
  const [activeTab, setActiveTab] = useState<TabType>('overdue');
  const [loading, setLoading] = useState(false);
  const [exporting, setExporting] = useState(false);
  const [filters, setFilters] = useState({ fromDate: '', toDate: '', branchId: '' });
  const [page, setPage] = useState(1);
  const [overdueData, setOverdueData] = useState<PagedResult<OverdueReportDto> | null>(null);
  const [popularData, setPopularData] = useState<PagedResult<PopularBookDto> | null>(null);
  const [activityData, setActivityData] = useState<PagedResult<MemberActivityDto> | null>(null);

  useEffect(() => { fetchData(); }, [activeTab, page, filters]);

  const fetchData = async () => {
    setLoading(true);
    try {
      if (activeTab === 'overdue') { const result = await reportService.getOverdueReport({ ...filters, page, pageSize: 20 }); setOverdueData(result); }
      else if (activeTab === 'popular') { const result = await reportService.getPopularBooks({ ...filters, page, pageSize: 20 }); setPopularData(result); }
      else if (activeTab === 'activity') { const result = await reportService.getMemberActivity({ ...filters, page, pageSize: 20 }); setActivityData(result); }
    } catch (error) { console.error('Failed to fetch report data', error); }
    finally { setLoading(false); }
  };

  const handleExport = async (format: 'excel' | 'pdf') => {
    if (activeTab !== 'overdue') return;
    setExporting(true);
    try { await reportService.exportOverdueReport(filters, format); }
    catch { alert('Failed to export report.'); }
    finally { setExporting(false); }
  };

  const renderPagination = (data: any) => {
    if (!data || data.totalPages <= 1) return null;
    return (
      <div className="px-6 py-3 border-t border-slate-800 bg-slate-900/50 flex items-center justify-between">
        <div className="text-sm text-slate-400">Showing <span className="font-medium text-white">{(page - 1) * 20 + 1}</span> to <span className="font-medium text-white">{Math.min(page * 20, data.totalCount)}</span> of <span className="font-medium text-white">{data.totalCount}</span> results</div>
        <div className="flex space-x-2">
          <button onClick={() => setPage(p => Math.max(1, p - 1))} disabled={!data.hasPreviousPage} className="px-3 py-1 bg-slate-800 text-slate-300 rounded hover:bg-slate-700 disabled:opacity-50 text-sm">Previous</button>
          <button onClick={() => setPage(p => Math.min(data.totalPages, p + 1))} disabled={!data.hasNextPage} className="px-3 py-1 bg-slate-800 text-slate-300 rounded hover:bg-slate-700 disabled:opacity-50 text-sm">Next</button>
        </div>
      </div>
    );
  };

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold tracking-tight text-white">Reports &amp; Analytics</h1>
        <p className="text-sm text-slate-400 mt-1">Generate insights and export library data.</p>
      </div>

      <ReportFilterBar onFilterChange={(f) => { setFilters(f as any); setPage(1); }} showBranchFilter={activeTab !== 'activity'} />

      <div className="bg-slate-900 border border-slate-800 rounded-2xl overflow-hidden shadow-sm">
        <div className="flex border-b border-slate-800">
          {(['overdue', 'popular', 'activity'] as TabType[]).map((tab) => (
            <button key={tab} onClick={() => { setActiveTab(tab); setPage(1); }} className={`flex-1 px-4 py-4 text-sm font-medium text-center transition-colors border-b-2 ${activeTab === tab ? 'border-indigo-500 text-indigo-400 bg-indigo-500/5' : 'border-transparent text-slate-400 hover:text-slate-300 hover:bg-slate-800/50'}`}>
              {tab === 'overdue' ? 'Overdue Books' : tab === 'popular' ? 'Most Popular Books' : 'Member Activity'}
            </button>
          ))}
        </div>

        {activeTab === 'overdue' && (
          <div className="px-6 py-3 border-b border-slate-800 bg-slate-900/50 flex justify-end gap-2">
            <button onClick={() => handleExport('excel')} disabled={exporting || !overdueData || overdueData.items.length === 0} className="px-3 py-1.5 bg-emerald-600/20 hover:bg-emerald-600/30 text-emerald-400 rounded-lg text-xs font-medium disabled:opacity-50 flex items-center gap-1.5 border border-emerald-500/20">Export Excel</button>
            <button onClick={() => handleExport('pdf')} disabled={exporting || !overdueData || overdueData.items.length === 0} className="px-3 py-1.5 bg-red-600/20 hover:bg-red-600/30 text-red-400 rounded-lg text-xs font-medium disabled:opacity-50 flex items-center gap-1.5 border border-red-500/20">Export PDF</button>
          </div>
        )}

        <div className="overflow-x-auto min-h-[400px]">
          {loading ? (
            <div className="flex justify-center py-20"><div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-500"></div></div>
          ) : activeTab === 'overdue' ? (
            <table className="w-full text-left text-sm text-slate-300">
              <thead className="text-xs uppercase bg-slate-900 border-b border-slate-800 text-slate-400">
                <tr><th className="px-6 py-4 font-medium">Member</th><th className="px-6 py-4 font-medium">Book &amp; Branch</th><th className="px-6 py-4 font-medium">Due Date</th><th className="px-6 py-4 font-medium text-right">Overdue</th><th className="px-6 py-4 font-medium text-right">Fine</th></tr>
              </thead>
              <tbody>
                {!overdueData || overdueData.items.length === 0 ? (<tr><td colSpan={5} className="px-6 py-10 text-center text-slate-500">No overdue records found.</td></tr>) : (
                  overdueData.items.map(row => (
                    <tr key={row.borrowId} className="border-b border-slate-800/50 hover:bg-slate-800/20">
                      <td className="px-6 py-4"><div className="font-medium text-white">{row.memberName}</div><div className="text-xs text-slate-500">{row.membershipNumber}</div></td>
                      <td className="px-6 py-4"><div className="text-slate-300">{row.bookTitle}</div><div className="text-xs text-slate-500">{row.branchName}</div></td>
                      <td className="px-6 py-4 text-slate-400">{new Date(row.dueDate).toLocaleDateString()}</td>
                      <td className="px-6 py-4 text-right"><span className="inline-flex px-2 py-1 rounded bg-red-500/10 text-red-400 text-xs font-bold border border-red-500/20">{row.overdueDays} days</span></td>
                      <td className="px-6 py-4 text-right font-medium text-white">${row.accruedFine.toFixed(2)}</td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          ) : activeTab === 'popular' ? (
            <table className="w-full text-left text-sm text-slate-300">
              <thead className="text-xs uppercase bg-slate-900 border-b border-slate-800 text-slate-400">
                <tr><th className="px-6 py-4 font-medium">Rank</th><th className="px-6 py-4 font-medium">Book Title</th><th className="px-6 py-4 font-medium">Author</th><th className="px-6 py-4 font-medium">Category</th><th className="px-6 py-4 font-medium text-right">Total Borrows</th></tr>
              </thead>
              <tbody>
                {!popularData || popularData.items.length === 0 ? (<tr><td colSpan={5} className="px-6 py-10 text-center text-slate-500">No borrow records found.</td></tr>) : (
                  popularData.items.map((row, index) => (
                    <tr key={row.bookId} className="border-b border-slate-800/50 hover:bg-slate-800/20">
                      <td className="px-6 py-4 font-bold text-slate-500">#{((page - 1) * 20) + index + 1}</td>
                      <td className="px-6 py-4 font-medium text-white">{row.title}</td>
                      <td className="px-6 py-4 text-slate-400">{row.authorName}</td>
                      <td className="px-6 py-4 text-slate-400">{row.categoryName}</td>
                      <td className="px-6 py-4 text-right"><span className="inline-flex items-center justify-center min-w-[2rem] h-6 px-2 rounded-full bg-indigo-500/10 text-indigo-400 text-xs font-bold border border-indigo-500/20">{row.totalBorrows}</span></td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          ) : (
            <table className="w-full text-left text-sm text-slate-300">
              <thead className="text-xs uppercase bg-slate-900 border-b border-slate-800 text-slate-400">
                <tr><th className="px-6 py-4 font-medium">Member</th><th className="px-6 py-4 font-medium text-center">Total Borrows</th><th className="px-6 py-4 font-medium text-center">Active</th><th className="px-6 py-4 font-medium text-center">Overdue</th><th className="px-6 py-4 font-medium text-right">Fines Paid</th></tr>
              </thead>
              <tbody>
                {!activityData || activityData.items.length === 0 ? (<tr><td colSpan={5} className="px-6 py-10 text-center text-slate-500">No member activity found.</td></tr>) : (
                  activityData.items.map((row) => (
                    <tr key={row.memberId} className="border-b border-slate-800/50 hover:bg-slate-800/20">
                      <td className="px-6 py-4"><div className="font-medium text-white">{row.fullName}</div><div className="text-xs text-slate-500">{row.membershipNumber}</div></td>
                      <td className="px-6 py-4 text-center font-medium text-slate-300">{row.totalBorrows}</td>
                      <td className="px-6 py-4 text-center text-indigo-400">{row.activeBorrows}</td>
                      <td className="px-6 py-4 text-center">{row.overdueBorrows > 0 ? <span className="text-red-400 font-bold">{row.overdueBorrows}</span> : <span className="text-slate-500">0</span>}</td>
                      <td className="px-6 py-4 text-right font-medium text-emerald-400">${row.totalFinesPaid.toFixed(2)}</td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          )}
        </div>

        {activeTab === 'overdue' && renderPagination(overdueData)}
        {activeTab === 'popular' && renderPagination(popularData)}
        {activeTab === 'activity' && renderPagination(activityData)}
      </div>
    </div>
  );
}
