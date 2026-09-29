import { useEffect, useState } from 'react'
import {
  Alert, Box, Button, Card, CardContent, CircularProgress, Stack, TextField, ToggleButton, ToggleButtonGroup, Typography,
} from '@mui/material'
import { Link as RouterLink, useNavigate } from 'react-router-dom'
import { appointmentsApi } from '../../api/appointments'
import { relationshipsApi } from '../../api/relationships'
import type { AppointmentChannel, Relationship } from '../../api/types'
import { ErrorAlert } from '../../components/ErrorAlert'
import { SlotPicker } from '../../components/SlotPicker'
import { channelLabel, formatDay, formatTime } from '../../utils/format'

export function BookAppointmentPage() {
  const navigate = useNavigate()
  const [relationship, setRelationship] = useState<Relationship | null>()
  const [slot, setSlot] = useState<string | null>(null)
  const [channel, setChannel] = useState<AppointmentChannel>('Call')
  const [reason, setReason] = useState('')
  const [error, setError] = useState<unknown>(null)
  const [submitting, setSubmitting] = useState(false)

  useEffect(() => {
    relationshipsApi.current().then((current) => setRelationship(current ?? null)).catch(setError)
  }, [])

  const hasManager = relationship?.status === 'Active'

  if (relationship === undefined) return error ? <ErrorAlert error={error} /> : <CircularProgress aria-label="Loading" />
  if (!hasManager) return <NoManagerYet relationship={relationship} />

  const { manager, customer } = relationship

  async function confirm() {
    if (!slot) return
    setSubmitting(true)
    setError(null)
    try {
      await appointmentsApi.book({ startsAt: slot, channel, reason })
      navigate('/customer')
    } catch (err) {
      setError(err)
      setSubmitting(false)
    }
  }

  return (
    <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: '1fr 340px' }, gap: 4, alignItems: 'start' }}>
      <Stack spacing={2.5}>
        <Typography variant="h1">Book an appointment with {manager.firstName}</Typography>
        <ErrorAlert error={error} />

        <Card>
          <CardContent sx={{ p: 3 }}>
            <Typography sx={{ fontWeight: 700, mb: 2.5 }}>1. When would you like to meet?</Typography>
            <SlotPicker slot={slot} onSlotChange={setSlot} onError={setError} />
          </CardContent>
        </Card>

        <Card>
          <CardContent sx={{ p: 3 }}>
            <Stack spacing={2}>
              <Typography sx={{ fontWeight: 700 }}>2. How would you like to meet?</Typography>
              <ToggleButtonGroup exclusive fullWidth color="primary" value={channel} onChange={(_, value) => value && setChannel(value)}>
                <ToggleButton value="Call">Call</ToggleButton>
                <ToggleButton value="BranchVisit">Branch visit</ToggleButton>
              </ToggleButtonGroup>
              <Typography color="text.secondary">
                {channel === 'Call'
                  ? `${manager.firstName} will call you on ${customer.phone}`
                  : `Meet at the ${manager.branch} branch`}
              </Typography>
              <TextField
                label="What would you like to discuss?"
                value={reason}
                onChange={(e) => setReason(e.target.value)}
                slotProps={{ htmlInput: { maxLength: 200 } }}
              />
            </Stack>
          </CardContent>
        </Card>
      </Stack>

      <Card sx={{ mt: { md: 7 } }}>
        <CardContent sx={{ p: 3 }}>
          <Stack spacing={2}>
            <Typography variant="h3">Summary</Typography>
            <SummaryRow label="When" value={slot ? `${formatDay(slot)}, ${formatTime(slot)}` : 'Pick a time'} />
            <SummaryRow label="How" value={channelLabel(channel)} />
            <SummaryRow label="With" value={`${manager.firstName} ${manager.lastName}`} />
            <SummaryRow label="About" value={reason || '—'} />
            <Button variant="contained" size="large" disabled={!slot || !reason.trim() || submitting} onClick={confirm}>
              {submitting ? 'Booking…' : 'Confirm booking'}
            </Button>
          </Stack>
        </CardContent>
      </Card>
    </Box>
  )
}

/** Booking needs an active manager: explain why and point to the next step. */
function NoManagerYet({ relationship }: { relationship: Relationship | null }) {
  return (
    <Stack spacing={2.5} sx={{ maxWidth: 560 }}>
      <Typography variant="h1">Book an appointment</Typography>
      <Alert severity="info">
        {relationship
          ? `Your request to ${relationship.manager.firstName} is still pending. You can book once they accept.`
          : 'You need a relationship manager before you can book an appointment.'}
      </Alert>
      <Box>
        <Button variant="contained" component={RouterLink} to="/customer">
          {relationship ? 'View my request' : 'Choose a manager'}
        </Button>
      </Box>
    </Stack>
  )
}

function SummaryRow({ label, value }: { label: string; value: string }) {
  return (
    <Stack direction="row" sx={{ justifyContent: 'space-between', gap: 2 }}>
      <Typography color="text.secondary">{label}</Typography>
      <Typography sx={{ fontWeight: 700, textAlign: 'right' }}>{value}</Typography>
    </Stack>
  )
}
