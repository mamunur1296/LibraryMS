import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { BookDto } from "../types/book.types";
import { favouriteService } from "../lib/services/favourite.service";
import { publicService } from "../lib/services/public.service";
import { useAuth } from "../contexts/AuthContext";
import { BookCard } from "../components/ui/BookCard";
import { Spinner } from "../components/ui/Spinner";
import { EmptyState } from "../components/ui/EmptyState";
import { Heart } from "lucide-react";
import { Button } from "../components/ui/Button";

export default function FavouritesPage() {
  const [books, setBooks] = useState<BookDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const { user } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    const fetchFavourites = async () => {
      setIsLoading(true);
      try {
        if (!user) {
          // Get from local storage
          const bookIds = favouriteService.getLocalFavourites();
          if (bookIds.length === 0) {
            setBooks([]);
            setIsLoading(false);
            return;
          }

          // Fetch book details for each id (in a real app, might want a batch endpoint)
          // Doing it one by one for now since it's local
          const bookPromises = bookIds.map((id: string) => 
            publicService.getBookById(id).catch(() => null)
          );
          
          const results = await Promise.all(bookPromises);
          setBooks(results.filter((b: any): b is BookDto => b !== null));
        } else {
          // Get from API
          const data = await favouriteService.getFavourites();
          setBooks(data.map((item: any) => item.book));
        }
      } catch (error) {
        console.error("Failed to fetch favourites", error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchFavourites();
  }, [user]);

  if (isLoading) {
    return (
      <div className="flex min-h-[500px] items-center justify-center">
        <Spinner size="lg" />
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8 max-w-7xl">
      <div className="mb-8 flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight flex items-center">
            <Heart className="mr-3 h-8 w-8 text-red-500 fill-red-500" />
            My Favourites
          </h1>
          <p className="text-muted-foreground mt-1">
            Books you've saved for later
          </p>
        </div>
      </div>

      {books.length > 0 ? (
<<<<<<< Updated upstream
        <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4">
          {books.map((book) => (
            <BookCard 
              key={book.id} 
              book={book} 
              onLoginPrompt={() => navigate("/login")}
            />
          ))}
=======
        <div className="overflow-x-auto rounded-xl border border-slate-800 bg-slate-900/50">
          <table className="w-full text-left text-sm text-slate-300">
            <thead className="bg-slate-900 text-xs uppercase text-slate-400 border-b border-slate-800">
              <tr>
                <th className="px-6 py-4 font-medium">Book Title</th>
                <th className="px-6 py-4 font-medium">Author</th>
                <th className="px-6 py-4 font-medium text-center">ISBN</th>
                <th className="px-6 py-4 font-medium text-center">Available Copies</th>
                <th className="px-6 py-4 font-medium text-right">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-800/50">
              {books.map((book) => (
                <tr key={book.id} className="hover:bg-slate-800/30 transition-colors">
                  <td className="px-6 py-4 font-medium text-white">{book.title}</td>
                  <td className="px-6 py-4">{book.authorName}</td>
                  <td className="px-6 py-4 text-center">{book.isbn}</td>
                  <td className="px-6 py-4 text-center">
                    {book.availableCopies > 0 ? (
                      <span className="text-emerald-400 font-medium">{book.availableCopies} available</span>
                    ) : (
                      <span className="text-rose-400">Out of stock</span>
                    )}
                  </td>
                  <td className="px-6 py-4 text-right">
                    <div className="flex justify-end gap-2">
                      <Button 
                        size="sm" 
                        variant="default"
                        className="bg-indigo-600 hover:bg-indigo-500"
                        disabled={book.availableCopies === 0}
                        onClick={() => navigate(`/borrows?action=new&bookId=${book.id}`)}
                      >
                        Borrow
                      </Button>
                      <Button 
                        size="sm" 
                        variant="destructive"
                        className="bg-rose-500/10 text-rose-500 hover:bg-rose-500 hover:text-white border border-rose-500/20"
                        onClick={async () => {
                          try {
                            if (user) {
                              await favouriteService.removeFavourite(book.id);
                            }
                            favouriteService.toggleLocalFavourite(book.id);
                            setBooks(books.filter(b => b.id !== book.id));
                          } catch (err) {
                            console.error("Failed to remove favourite", err);
                          }
                        }}
                      >
                        Unfavourite
                      </Button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
>>>>>>> Stashed changes
        </div>
      ) : (
        <EmptyState
          icon={<Heart className="h-10 w-10 text-muted-foreground" />}
          title="No favourites yet"
          description="You haven't saved any books to your favourites. Browse the catalog to find books you love."
          action={
            <Button onClick={() => navigate("/catalog")}>
              Browse Catalog
            </Button>
          }
        />
      )}
    </div>
  );
}
