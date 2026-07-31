import { Outlet, Link } from "react-router-dom";
import { ThemeToggle } from "../ui/ThemeToggle";

export function PublicLayout() {
  return (
    <div className="min-h-screen bg-background text-foreground flex flex-col">
      <header className="sticky top-0 z-40 w-full border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
        <div className="container flex h-16 items-center justify-between mx-auto px-4">
          <div className="flex gap-6 md:gap-10">
            <Link to="/" className="flex items-center space-x-2">
              <span className="inline-block font-bold text-xl">
                Library<span className="text-primary">MS</span>
              </span>
            </Link>
            <nav className="hidden md:flex gap-6">
              <Link
                to="/catalog"
                className="flex items-center text-sm font-medium text-muted-foreground transition-colors hover:text-foreground"
              >
                Catalog
              </Link>
              <Link
                to="/about"
                className="flex items-center text-sm font-medium text-muted-foreground transition-colors hover:text-foreground"
              >
                About
              </Link>
            </nav>
          </div>
          <div className="flex items-center gap-4">
            <ThemeToggle />
            <Link
              to="/login"
              className="inline-flex h-9 items-center justify-center rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground shadow transition-colors hover:bg-primary/90 focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring"
            >
              Sign In
            </Link>
          </div>
        </div>
      </header>

      <main className="flex-1">
        <Outlet />
      </main>

      <footer className="border-t py-6 md:py-0">
        <div className="container flex flex-col items-center justify-between gap-4 md:h-16 md:flex-row mx-auto px-4 text-sm text-muted-foreground text-center md:text-left">
          <p>
            &copy; {new Date().getFullYear()} LibraryMS. All rights reserved.
          </p>
          <div className="flex gap-4">
            <Link to="/terms" className="hover:underline">Terms</Link>
            <Link to="/privacy" className="hover:underline">Privacy</Link>
          </div>
        </div>
      </footer>
    </div>
  );
}
