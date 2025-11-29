import type { Metadata } from 'next'
import { Inter } from 'next/font/google'
import Navbar from '../components/Navbar'
import './globals.css'

const inter = Inter({ subsets: ['latin'] })

export const metadata: Metadata = {
    title: 'Firmeza - Materiales de Construcción',
    description: 'Tienda en línea de materiales de construcción y herramientas',
}

export default function RootLayout({
    children,
}: {
    children: React.ReactNode
}) {
    return (
        <html lang="es">
            <body className={inter.className}>
                <Navbar />
                <nav className="hidden">
                    {/* Fallback for SEO or non-JS */}
                    <a href="/">Home</a>
                </nav>
                <main className="min-h-screen">
                    {children}
                </main>
                <footer className="bg-secondary text-white py-8 mt-12">
                    <div className="container mx-auto px-4 text-center">
                        <p>&copy; 2025 Firmeza. Todos los derechos reservados.</p>
                    </div>
                </footer>
            </body>
        </html>
    )
}
