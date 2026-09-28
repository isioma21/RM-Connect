import { useEffect, useState } from 'react'
import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from '@mui/material'
import { useNavigate } from 'react-router-dom'
import { authApi } from '../api/auth'
import { lastApiSuccessAt } from '../api/client'
import { useAuth } from '../auth/AuthContext'

// Matches Auth:SessionMinutes on the API
const sessionMinutes = 30
const warnMinutes = 25

/** Warns before the session runs out and lets the user stay signed in. */
export function IdleWarning() {
  const { logout } = useAuth()
  const navigate = useNavigate()
  const [secondsLeft, setSecondsLeft] = useState<number | null>(null)

  useEffect(() => {
    const timer = setInterval(() => {
      const idleSeconds = (Date.now() - lastApiSuccessAt()) / 1000
      const left = Math.ceil(sessionMinutes * 60 - idleSeconds)

      if (left <= 0) {
        clearInterval(timer)
        authApi.logout().finally(() => window.location.assign('/login?signedOut=1'))
      } else {
        setSecondsLeft(idleSeconds >= warnMinutes * 60 ? left : null)
      }
    }, 1000)
    return () => clearInterval(timer)
  }, [])

  async function staySignedIn() {
    await authApi.currentUser().catch(() => {})
    setSecondsLeft(null)
  }

  const minutes = Math.floor((secondsLeft ?? 0) / 60)
  const seconds = String((secondsLeft ?? 0) % 60).padStart(2, '0')

  return (
    <Dialog open={secondsLeft !== null} maxWidth="xs" fullWidth>
      <DialogTitle sx={{ fontFamily: "'Fraunces', Georgia, serif", fontWeight: 600 }}>Still there?</DialogTitle>
      <DialogContent>
        <DialogContentText>
          You will be signed out in {minutes}:{seconds} because you haven't been active.
        </DialogContentText>
      </DialogContent>
      <DialogActions sx={{ px: 3, pb: 2.5 }}>
        <Button onClick={() => logout().then(() => navigate('/login'))} color="inherit" variant="outlined">Log out</Button>
        <Button onClick={staySignedIn} variant="contained">Stay signed in</Button>
      </DialogActions>
    </Dialog>
  )
}
