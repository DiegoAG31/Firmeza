import type { Metadata } from 'next'
import { Inter } from 'next/font/google'
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
                <nav className="bg-secondary text-white shadow-lg">
                    <div className="container mx-auto px-4 py-4">
                        <div className="flex justify-between items-center">
                            <a href="/" className="text-2xl font-bold text-primary">
                                🏗️ FIRMEZA
                            </a>
                            <div className="flex gap-4">
                                <a href="/" className="hover:text-primary transition">Productos</a>
                                <a href="/cart" className="hover:text-primary transition">Carrito</a>
                                <a href="/login" className="hover:text-primary transition">Ingresar</a>
                            </div>
                        </div>
                    </div>
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
