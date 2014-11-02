/*=============================================================================
  Copyright (C) 2013 Allied Vision Technologies.  All Rights Reserved.

  Redistribution of this file, in original or modified form, without
  prior written consent of Allied Vision Technologies is prohibited.

-------------------------------------------------------------------------------

  Please do not modify this file, because it was created automatically by a 
  code generator tool (AVT VimbaClassGenerator). So any manual modifications 
  will be lost if you run the tool again.

-------------------------------------------------------------------------------

  THIS SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS" AND ANY EXPRESS OR IMPLIED
  WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF TITLE,
  NON-INFRINGEMENT, MERCHANTABILITY AND FITNESS FOR A PARTICULAR  PURPOSE ARE
  DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT,
  INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED
  AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR
  TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
  OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

=============================================================================*/

namespace MT3
{
    public class GE680CCamera : AVT.VmbAPINET.Camera
    {
        #region Constructors.

        public GE680CCamera (
            string cameraID,
            string cameraName,
            string cameraModel,
            string cameraSerialNumber,
            string interfaceID,
            AVT.VmbAPINET.VmbInterfaceType interfaceType,
            string interfaceName,
            string interfaceSerialNumber,
            AVT.VmbAPINET.VmbAccessModeType interfacePermittedAccess)
            : base (
                cameraID,
                cameraName,
                cameraModel,
                cameraSerialNumber,
                interfaceID,
                interfaceType,
                interfaceName,
                interfaceSerialNumber,
                interfacePermittedAccess)
        {
        }

        #endregion

        #region Public properties.

        #region Category /AcquisitionControl

        public long AcquisitionFrameCount
        {
            get { return AcquisitionFrameCountFeature.IntValue; }
            set { AcquisitionFrameCountFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature AcquisitionFrameCountFeature
        {
            get
            {
                if (m_AcquisitionFrameCountFeature == null)
                    m_AcquisitionFrameCountFeature = Features ["AcquisitionFrameCount"];
                return m_AcquisitionFrameCountFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_AcquisitionFrameCountFeature = null;

        public double AcquisitionFrameRateAbs
        {
            get { return AcquisitionFrameRateAbsFeature.FloatValue; }
            set { AcquisitionFrameRateAbsFeature.FloatValue = value; }
        }
        public AVT.VmbAPINET.Feature AcquisitionFrameRateAbsFeature
        {
            get
            {
                if (m_AcquisitionFrameRateAbsFeature == null)
                    m_AcquisitionFrameRateAbsFeature = Features ["AcquisitionFrameRateAbs"];
                return m_AcquisitionFrameRateAbsFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_AcquisitionFrameRateAbsFeature = null;

        public double AcquisitionFrameRateLimit
        {
            get { return AcquisitionFrameRateLimitFeature.FloatValue; }
        }
        public AVT.VmbAPINET.Feature AcquisitionFrameRateLimitFeature
        {
            get
            {
                if (m_AcquisitionFrameRateLimitFeature == null)
                    m_AcquisitionFrameRateLimitFeature = Features ["AcquisitionFrameRateLimit"];
                return m_AcquisitionFrameRateLimitFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_AcquisitionFrameRateLimitFeature = null;

        public AcquisitionModeEnum AcquisitionMode
        {
            get { return (AcquisitionModeEnum) AcquisitionModeFeature.EnumIntValue; }
            set { AcquisitionModeFeature.EnumIntValue = (int) value; }
        }
        public AVT.VmbAPINET.Feature AcquisitionModeFeature
        {
            get
            {
                if (m_AcquisitionModeFeature == null)
                    m_AcquisitionModeFeature = Features ["AcquisitionMode"];
                return m_AcquisitionModeFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_AcquisitionModeFeature = null;

        public long RecorderPreEventCount
        {
            get { return RecorderPreEventCountFeature.IntValue; }
            set { RecorderPreEventCountFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature RecorderPreEventCountFeature
        {
            get
            {
                if (m_RecorderPreEventCountFeature == null)
                    m_RecorderPreEventCountFeature = Features ["RecorderPreEventCount"];
                return m_RecorderPreEventCountFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_RecorderPreEventCountFeature = null;

        public TriggerActivationEnum TriggerActivation
        {
            get { return (TriggerActivationEnum) TriggerActivationFeature.EnumIntValue; }
            set { TriggerActivationFeature.EnumIntValue = (int) value; }
        }
        public AVT.VmbAPINET.Feature TriggerActivationFeature
        {
            get
            {
                if (m_TriggerActivationFeature == null)
                    m_TriggerActivationFeature = Features ["TriggerActivation"];
                return m_TriggerActivationFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_TriggerActivationFeature = null;

        public double TriggerDelayAbs
        {
            get { return TriggerDelayAbsFeature.FloatValue; }
            set { TriggerDelayAbsFeature.FloatValue = value; }
        }
        public AVT.VmbAPINET.Feature TriggerDelayAbsFeature
        {
            get
            {
                if (m_TriggerDelayAbsFeature == null)
                    m_TriggerDelayAbsFeature = Features ["TriggerDelayAbs"];
                return m_TriggerDelayAbsFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_TriggerDelayAbsFeature = null;

        public TriggerModeEnum TriggerMode
        {
            get { return (TriggerModeEnum) TriggerModeFeature.EnumIntValue; }
            set { TriggerModeFeature.EnumIntValue = (int) value; }
        }
        public AVT.VmbAPINET.Feature TriggerModeFeature
        {
            get
            {
                if (m_TriggerModeFeature == null)
                    m_TriggerModeFeature = Features ["TriggerMode"];
                return m_TriggerModeFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_TriggerModeFeature = null;

        public TriggerSelectorEnum TriggerSelector
        {
            get { return (TriggerSelectorEnum) TriggerSelectorFeature.EnumIntValue; }
            set { TriggerSelectorFeature.EnumIntValue = (int) value; }
        }
        public AVT.VmbAPINET.Feature TriggerSelectorFeature
        {
            get
            {
                if (m_TriggerSelectorFeature == null)
                    m_TriggerSelectorFeature = Features ["TriggerSelector"];
                return m_TriggerSelectorFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_TriggerSelectorFeature = null;

        public TriggerSourceEnum TriggerSource
        {
            get { return (TriggerSourceEnum) TriggerSourceFeature.EnumIntValue; }
            set { TriggerSourceFeature.EnumIntValue = (int) value; }
        }
        public AVT.VmbAPINET.Feature TriggerSourceFeature
        {
            get
            {
                if (m_TriggerSourceFeature == null)
                    m_TriggerSourceFeature = Features ["TriggerSource"];
                return m_TriggerSourceFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_TriggerSourceFeature = null;

        #endregion

        #region Category /AcquisitionControl/StreamHold

        public long StreamHoldCapacity
        {
            get { return StreamHoldCapacityFeature.IntValue; }
        }
        public AVT.VmbAPINET.Feature StreamHoldCapacityFeature
        {
            get
            {
                if (m_StreamHoldCapacityFeature == null)
                    m_StreamHoldCapacityFeature = Features ["StreamHoldCapacity"];
                return m_StreamHoldCapacityFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_StreamHoldCapacityFeature = null;

        public StreamHoldEnableEnum StreamHoldEnable
        {
            get { return (StreamHoldEnableEnum) StreamHoldEnableFeature.EnumIntValue; }
            set { StreamHoldEnableFeature.EnumIntValue = (int) value; }
        }
        public AVT.VmbAPINET.Feature StreamHoldEnableFeature
        {
            get
            {
                if (m_StreamHoldEnableFeature == null)
                    m_StreamHoldEnableFeature = Features ["StreamHoldEnable"];
                return m_StreamHoldEnableFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_StreamHoldEnableFeature = null;

        #endregion

        #region Category /Bandwidth

        public BandwidthControlModeEnum BandwidthControlMode
        {
            get { return (BandwidthControlModeEnum) BandwidthControlModeFeature.EnumIntValue; }
            set { BandwidthControlModeFeature.EnumIntValue = (int) value; }
        }
        public AVT.VmbAPINET.Feature BandwidthControlModeFeature
        {
            get
            {
                if (m_BandwidthControlModeFeature == null)
                    m_BandwidthControlModeFeature = Features ["BandwidthControlMode"];
                return m_BandwidthControlModeFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_BandwidthControlModeFeature = null;

        public long StreamBytesPerSecond
        {
            get { return StreamBytesPerSecondFeature.IntValue; }
            set { StreamBytesPerSecondFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature StreamBytesPerSecondFeature
        {
            get
            {
                if (m_StreamBytesPerSecondFeature == null)
                    m_StreamBytesPerSecondFeature = Features ["StreamBytesPerSecond"];
                return m_StreamBytesPerSecondFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_StreamBytesPerSecondFeature = null;

        #endregion

        #region Category /FeatureControl

        public double BalanceRatioAbs
        {
            get { return BalanceRatioAbsFeature.FloatValue; }
            set { BalanceRatioAbsFeature.FloatValue = value; }
        }
        public AVT.VmbAPINET.Feature BalanceRatioAbsFeature
        {
            get
            {
                if (m_BalanceRatioAbsFeature == null)
                    m_BalanceRatioAbsFeature = Features ["BalanceRatioAbs"];
                return m_BalanceRatioAbsFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_BalanceRatioAbsFeature = null;

        public BalanceRatioSelectorEnum BalanceRatioSelector
        {
            get { return (BalanceRatioSelectorEnum) BalanceRatioSelectorFeature.EnumIntValue; }
            set { BalanceRatioSelectorFeature.EnumIntValue = (int) value; }
        }
        public AVT.VmbAPINET.Feature BalanceRatioSelectorFeature
        {
            get
            {
                if (m_BalanceRatioSelectorFeature == null)
                    m_BalanceRatioSelectorFeature = Features ["BalanceRatioSelector"];
                return m_BalanceRatioSelectorFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_BalanceRatioSelectorFeature = null;

        public BalanceWhiteAutoEnum BalanceWhiteAuto
        {
            get { return (BalanceWhiteAutoEnum) BalanceWhiteAutoFeature.EnumIntValue; }
            set { BalanceWhiteAutoFeature.EnumIntValue = (int) value; }
        }
        public AVT.VmbAPINET.Feature BalanceWhiteAutoFeature
        {
            get
            {
                if (m_BalanceWhiteAutoFeature == null)
                    m_BalanceWhiteAutoFeature = Features ["BalanceWhiteAuto"];
                return m_BalanceWhiteAutoFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_BalanceWhiteAutoFeature = null;

        public ExposureAutoEnum ExposureAuto
        {
            get { return (ExposureAutoEnum) ExposureAutoFeature.EnumIntValue; }
            set { ExposureAutoFeature.EnumIntValue = (int) value; }
        }
        public AVT.VmbAPINET.Feature ExposureAutoFeature
        {
            get
            {
                if (m_ExposureAutoFeature == null)
                    m_ExposureAutoFeature = Features ["ExposureAuto"];
                return m_ExposureAutoFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_ExposureAutoFeature = null;

        public ExposureModeEnum ExposureMode
        {
            get { return (ExposureModeEnum) ExposureModeFeature.EnumIntValue; }
            set { ExposureModeFeature.EnumIntValue = (int) value; }
        }
        public AVT.VmbAPINET.Feature ExposureModeFeature
        {
            get
            {
                if (m_ExposureModeFeature == null)
                    m_ExposureModeFeature = Features ["ExposureMode"];
                return m_ExposureModeFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_ExposureModeFeature = null;

        public double ExposureTimeAbs
        {
            get { return ExposureTimeAbsFeature.FloatValue; }
            set { ExposureTimeAbsFeature.FloatValue = value; }
        }
        public AVT.VmbAPINET.Feature ExposureTimeAbsFeature
        {
            get
            {
                if (m_ExposureTimeAbsFeature == null)
                    m_ExposureTimeAbsFeature = Features ["ExposureTimeAbs"];
                return m_ExposureTimeAbsFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_ExposureTimeAbsFeature = null;

        public GainAutoEnum GainAuto
        {
            get { return (GainAutoEnum) GainAutoFeature.EnumIntValue; }
            set { GainAutoFeature.EnumIntValue = (int) value; }
        }
        public AVT.VmbAPINET.Feature GainAutoFeature
        {
            get
            {
                if (m_GainAutoFeature == null)
                    m_GainAutoFeature = Features ["GainAuto"];
                return m_GainAutoFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GainAutoFeature = null;

        public long GainRaw
        {
            get { return GainRawFeature.IntValue; }
            set { GainRawFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature GainRawFeature
        {
            get
            {
                if (m_GainRawFeature == null)
                    m_GainRawFeature = Features ["GainRaw"];
                return m_GainRawFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GainRawFeature = null;

        public GainSelectorEnum GainSelector
        {
            get { return (GainSelectorEnum) GainSelectorFeature.EnumIntValue; }
            set { GainSelectorFeature.EnumIntValue = (int) value; }
        }
        public AVT.VmbAPINET.Feature GainSelectorFeature
        {
            get
            {
                if (m_GainSelectorFeature == null)
                    m_GainSelectorFeature = Features ["GainSelector"];
                return m_GainSelectorFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GainSelectorFeature = null;

        #endregion

        #region Category /FeatureControl/BalanceWhiteAutoControl

        public long BalanceWhiteAutoAdjustTol
        {
            get { return BalanceWhiteAutoAdjustTolFeature.IntValue; }
            set { BalanceWhiteAutoAdjustTolFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature BalanceWhiteAutoAdjustTolFeature
        {
            get
            {
                if (m_BalanceWhiteAutoAdjustTolFeature == null)
                    m_BalanceWhiteAutoAdjustTolFeature = Features ["BalanceWhiteAutoAdjustTol"];
                return m_BalanceWhiteAutoAdjustTolFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_BalanceWhiteAutoAdjustTolFeature = null;

        public long BalanceWhiteAutoRate
        {
            get { return BalanceWhiteAutoRateFeature.IntValue; }
            set { BalanceWhiteAutoRateFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature BalanceWhiteAutoRateFeature
        {
            get
            {
                if (m_BalanceWhiteAutoRateFeature == null)
                    m_BalanceWhiteAutoRateFeature = Features ["BalanceWhiteAutoRate"];
                return m_BalanceWhiteAutoRateFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_BalanceWhiteAutoRateFeature = null;

        #endregion

        #region Category /FeatureControl/DSPSubregion

        public long DSPSubregionBottom
        {
            get { return DSPSubregionBottomFeature.IntValue; }
            set { DSPSubregionBottomFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature DSPSubregionBottomFeature
        {
            get
            {
                if (m_DSPSubregionBottomFeature == null)
                    m_DSPSubregionBottomFeature = Features ["DSPSubregionBottom"];
                return m_DSPSubregionBottomFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_DSPSubregionBottomFeature = null;

        public long DSPSubregionLeft
        {
            get { return DSPSubregionLeftFeature.IntValue; }
            set { DSPSubregionLeftFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature DSPSubregionLeftFeature
        {
            get
            {
                if (m_DSPSubregionLeftFeature == null)
                    m_DSPSubregionLeftFeature = Features ["DSPSubregionLeft"];
                return m_DSPSubregionLeftFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_DSPSubregionLeftFeature = null;

        public long DSPSubregionRight
        {
            get { return DSPSubregionRightFeature.IntValue; }
            set { DSPSubregionRightFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature DSPSubregionRightFeature
        {
            get
            {
                if (m_DSPSubregionRightFeature == null)
                    m_DSPSubregionRightFeature = Features ["DSPSubregionRight"];
                return m_DSPSubregionRightFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_DSPSubregionRightFeature = null;

        public long DSPSubregionTop
        {
            get { return DSPSubregionTopFeature.IntValue; }
            set { DSPSubregionTopFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature DSPSubregionTopFeature
        {
            get
            {
                if (m_DSPSubregionTopFeature == null)
                    m_DSPSubregionTopFeature = Features ["DSPSubregionTop"];
                return m_DSPSubregionTopFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_DSPSubregionTopFeature = null;

        #endregion

        #region Category /FeatureControl/ExposureAutoControl

        public long ExposureAutoAdjustTol
        {
            get { return ExposureAutoAdjustTolFeature.IntValue; }
            set { ExposureAutoAdjustTolFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature ExposureAutoAdjustTolFeature
        {
            get
            {
                if (m_ExposureAutoAdjustTolFeature == null)
                    m_ExposureAutoAdjustTolFeature = Features ["ExposureAutoAdjustTol"];
                return m_ExposureAutoAdjustTolFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_ExposureAutoAdjustTolFeature = null;

        public ExposureAutoAlgEnum ExposureAutoAlg
        {
            get { return (ExposureAutoAlgEnum) ExposureAutoAlgFeature.EnumIntValue; }
            set { ExposureAutoAlgFeature.EnumIntValue = (int) value; }
        }
        public AVT.VmbAPINET.Feature ExposureAutoAlgFeature
        {
            get
            {
                if (m_ExposureAutoAlgFeature == null)
                    m_ExposureAutoAlgFeature = Features ["ExposureAutoAlg"];
                return m_ExposureAutoAlgFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_ExposureAutoAlgFeature = null;

        public long ExposureAutoMax
        {
            get { return ExposureAutoMaxFeature.IntValue; }
            set { ExposureAutoMaxFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature ExposureAutoMaxFeature
        {
            get
            {
                if (m_ExposureAutoMaxFeature == null)
                    m_ExposureAutoMaxFeature = Features ["ExposureAutoMax"];
                return m_ExposureAutoMaxFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_ExposureAutoMaxFeature = null;

        public long ExposureAutoMin
        {
            get { return ExposureAutoMinFeature.IntValue; }
            set { ExposureAutoMinFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature ExposureAutoMinFeature
        {
            get
            {
                if (m_ExposureAutoMinFeature == null)
                    m_ExposureAutoMinFeature = Features ["ExposureAutoMin"];
                return m_ExposureAutoMinFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_ExposureAutoMinFeature = null;

        public long ExposureAutoOutliers
        {
            get { return ExposureAutoOutliersFeature.IntValue; }
            set { ExposureAutoOutliersFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature ExposureAutoOutliersFeature
        {
            get
            {
                if (m_ExposureAutoOutliersFeature == null)
                    m_ExposureAutoOutliersFeature = Features ["ExposureAutoOutliers"];
                return m_ExposureAutoOutliersFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_ExposureAutoOutliersFeature = null;

        public long ExposureAutoRate
        {
            get { return ExposureAutoRateFeature.IntValue; }
            set { ExposureAutoRateFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature ExposureAutoRateFeature
        {
            get
            {
                if (m_ExposureAutoRateFeature == null)
                    m_ExposureAutoRateFeature = Features ["ExposureAutoRate"];
                return m_ExposureAutoRateFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_ExposureAutoRateFeature = null;

        public long ExposureAutoTarget
        {
            get { return ExposureAutoTargetFeature.IntValue; }
            set { ExposureAutoTargetFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature ExposureAutoTargetFeature
        {
            get
            {
                if (m_ExposureAutoTargetFeature == null)
                    m_ExposureAutoTargetFeature = Features ["ExposureAutoTarget"];
                return m_ExposureAutoTargetFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_ExposureAutoTargetFeature = null;

        #endregion

        #region Category /FeatureControl/GainAutoControl

        public long GainAutoAdjustTol
        {
            get { return GainAutoAdjustTolFeature.IntValue; }
            set { GainAutoAdjustTolFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature GainAutoAdjustTolFeature
        {
            get
            {
                if (m_GainAutoAdjustTolFeature == null)
                    m_GainAutoAdjustTolFeature = Features ["GainAutoAdjustTol"];
                return m_GainAutoAdjustTolFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GainAutoAdjustTolFeature = null;

        public long GainAutoMax
        {
            get { return GainAutoMaxFeature.IntValue; }
            set { GainAutoMaxFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature GainAutoMaxFeature
        {
            get
            {
                if (m_GainAutoMaxFeature == null)
                    m_GainAutoMaxFeature = Features ["GainAutoMax"];
                return m_GainAutoMaxFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GainAutoMaxFeature = null;

        public long GainAutoMin
        {
            get { return GainAutoMinFeature.IntValue; }
            set { GainAutoMinFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature GainAutoMinFeature
        {
            get
            {
                if (m_GainAutoMinFeature == null)
                    m_GainAutoMinFeature = Features ["GainAutoMin"];
                return m_GainAutoMinFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GainAutoMinFeature = null;

        public long GainAutoOutliers
        {
            get { return GainAutoOutliersFeature.IntValue; }
            set { GainAutoOutliersFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature GainAutoOutliersFeature
        {
            get
            {
                if (m_GainAutoOutliersFeature == null)
                    m_GainAutoOutliersFeature = Features ["GainAutoOutliers"];
                return m_GainAutoOutliersFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GainAutoOutliersFeature = null;

        public long GainAutoRate
        {
            get { return GainAutoRateFeature.IntValue; }
            set { GainAutoRateFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature GainAutoRateFeature
        {
            get
            {
                if (m_GainAutoRateFeature == null)
                    m_GainAutoRateFeature = Features ["GainAutoRate"];
                return m_GainAutoRateFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GainAutoRateFeature = null;

        public long GainAutoTarget
        {
            get { return GainAutoTargetFeature.IntValue; }
            set { GainAutoTargetFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature GainAutoTargetFeature
        {
            get
            {
                if (m_GainAutoTargetFeature == null)
                    m_GainAutoTargetFeature = Features ["GainAutoTarget"];
                return m_GainAutoTargetFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GainAutoTargetFeature = null;

        #endregion

        #region Category /GigE

        public long GevDeviceMACAddress
        {
            get { return GevDeviceMACAddressFeature.IntValue; }
        }
        public AVT.VmbAPINET.Feature GevDeviceMACAddressFeature
        {
            get
            {
                if (m_GevDeviceMACAddressFeature == null)
                    m_GevDeviceMACAddressFeature = Features ["GevDeviceMACAddress"];
                return m_GevDeviceMACAddressFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GevDeviceMACAddressFeature = null;

        public long GevSCPSPacketSize
        {
            get { return GevSCPSPacketSizeFeature.IntValue; }
            set { GevSCPSPacketSizeFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature GevSCPSPacketSizeFeature
        {
            get
            {
                if (m_GevSCPSPacketSizeFeature == null)
                    m_GevSCPSPacketSizeFeature = Features ["GevSCPSPacketSize"];
                return m_GevSCPSPacketSizeFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GevSCPSPacketSizeFeature = null;

        #endregion

        #region Category /GigE/Configuration

        public GevIPConfigurationModeEnum GevIPConfigurationMode
        {
            get { return (GevIPConfigurationModeEnum) GevIPConfigurationModeFeature.EnumIntValue; }
        }
        public AVT.VmbAPINET.Feature GevIPConfigurationModeFeature
        {
            get
            {
                if (m_GevIPConfigurationModeFeature == null)
                    m_GevIPConfigurationModeFeature = Features ["GevIPConfigurationMode"];
                return m_GevIPConfigurationModeFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GevIPConfigurationModeFeature = null;

        #endregion

        #region Category /GigE/Current

        public long GevCurrentDefaultGateway
        {
            get { return GevCurrentDefaultGatewayFeature.IntValue; }
        }
        public AVT.VmbAPINET.Feature GevCurrentDefaultGatewayFeature
        {
            get
            {
                if (m_GevCurrentDefaultGatewayFeature == null)
                    m_GevCurrentDefaultGatewayFeature = Features ["GevCurrentDefaultGateway"];
                return m_GevCurrentDefaultGatewayFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GevCurrentDefaultGatewayFeature = null;

        public long GevCurrentIPAddress
        {
            get { return GevCurrentIPAddressFeature.IntValue; }
        }
        public AVT.VmbAPINET.Feature GevCurrentIPAddressFeature
        {
            get
            {
                if (m_GevCurrentIPAddressFeature == null)
                    m_GevCurrentIPAddressFeature = Features ["GevCurrentIPAddress"];
                return m_GevCurrentIPAddressFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GevCurrentIPAddressFeature = null;

        public long GevCurrentSubnetMask
        {
            get { return GevCurrentSubnetMaskFeature.IntValue; }
        }
        public AVT.VmbAPINET.Feature GevCurrentSubnetMaskFeature
        {
            get
            {
                if (m_GevCurrentSubnetMaskFeature == null)
                    m_GevCurrentSubnetMaskFeature = Features ["GevCurrentSubnetMask"];
                return m_GevCurrentSubnetMaskFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GevCurrentSubnetMaskFeature = null;

        #endregion

        #region Category /GigE/GVCP

        public long GVCPCmdRetries
        {
            get { return GVCPCmdRetriesFeature.IntValue; }
            set { GVCPCmdRetriesFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature GVCPCmdRetriesFeature
        {
            get
            {
                if (m_GVCPCmdRetriesFeature == null)
                    m_GVCPCmdRetriesFeature = Features ["GVCPCmdRetries"];
                return m_GVCPCmdRetriesFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GVCPCmdRetriesFeature = null;

        public long GVCPCmdTimeout
        {
            get { return GVCPCmdTimeoutFeature.IntValue; }
            set { GVCPCmdTimeoutFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature GVCPCmdTimeoutFeature
        {
            get
            {
                if (m_GVCPCmdTimeoutFeature == null)
                    m_GVCPCmdTimeoutFeature = Features ["GVCPCmdTimeout"];
                return m_GVCPCmdTimeoutFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GVCPCmdTimeoutFeature = null;

        public long GVCPHBInterval
        {
            get { return GVCPHBIntervalFeature.IntValue; }
            set { GVCPHBIntervalFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature GVCPHBIntervalFeature
        {
            get
            {
                if (m_GVCPHBIntervalFeature == null)
                    m_GVCPHBIntervalFeature = Features ["GVCPHBInterval"];
                return m_GVCPHBIntervalFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GVCPHBIntervalFeature = null;

        #endregion

        #region Category /GigE/Persistent

        public long GevPersistentDefaultGateway
        {
            get { return GevPersistentDefaultGatewayFeature.IntValue; }
        }
        public AVT.VmbAPINET.Feature GevPersistentDefaultGatewayFeature
        {
            get
            {
                if (m_GevPersistentDefaultGatewayFeature == null)
                    m_GevPersistentDefaultGatewayFeature = Features ["GevPersistentDefaultGateway"];
                return m_GevPersistentDefaultGatewayFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GevPersistentDefaultGatewayFeature = null;

        public long GevPersistentIPAddress
        {
            get { return GevPersistentIPAddressFeature.IntValue; }
        }
        public AVT.VmbAPINET.Feature GevPersistentIPAddressFeature
        {
            get
            {
                if (m_GevPersistentIPAddressFeature == null)
                    m_GevPersistentIPAddressFeature = Features ["GevPersistentIPAddress"];
                return m_GevPersistentIPAddressFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GevPersistentIPAddressFeature = null;

        public long GevPersistentSubnetMask
        {
            get { return GevPersistentSubnetMaskFeature.IntValue; }
        }
        public AVT.VmbAPINET.Feature GevPersistentSubnetMaskFeature
        {
            get
            {
                if (m_GevPersistentSubnetMaskFeature == null)
                    m_GevPersistentSubnetMaskFeature = Features ["GevPersistentSubnetMask"];
                return m_GevPersistentSubnetMaskFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GevPersistentSubnetMaskFeature = null;

        #endregion

        #region Category /ImageFormat

        public long Height
        {
            get { return HeightFeature.IntValue; }
            set { HeightFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature HeightFeature
        {
            get
            {
                if (m_HeightFeature == null)
                    m_HeightFeature = Features ["Height"];
                return m_HeightFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_HeightFeature = null;

        public long HeightMax
        {
            get { return HeightMaxFeature.IntValue; }
        }
        public AVT.VmbAPINET.Feature HeightMaxFeature
        {
            get
            {
                if (m_HeightMaxFeature == null)
                    m_HeightMaxFeature = Features ["HeightMax"];
                return m_HeightMaxFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_HeightMaxFeature = null;

        public long OffsetX
        {
            get { return OffsetXFeature.IntValue; }
            set { OffsetXFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature OffsetXFeature
        {
            get
            {
                if (m_OffsetXFeature == null)
                    m_OffsetXFeature = Features ["OffsetX"];
                return m_OffsetXFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_OffsetXFeature = null;

        public long OffsetY
        {
            get { return OffsetYFeature.IntValue; }
            set { OffsetYFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature OffsetYFeature
        {
            get
            {
                if (m_OffsetYFeature == null)
                    m_OffsetYFeature = Features ["OffsetY"];
                return m_OffsetYFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_OffsetYFeature = null;

        public long PayloadSize
        {
            get { return PayloadSizeFeature.IntValue; }
        }
        public AVT.VmbAPINET.Feature PayloadSizeFeature
        {
            get
            {
                if (m_PayloadSizeFeature == null)
                    m_PayloadSizeFeature = Features ["PayloadSize"];
                return m_PayloadSizeFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_PayloadSizeFeature = null;

        public PixelFormatEnum PixelFormat
        {
            get { return (PixelFormatEnum) PixelFormatFeature.EnumIntValue; }
            set { PixelFormatFeature.EnumIntValue = (int) value; }
        }
        public AVT.VmbAPINET.Feature PixelFormatFeature
        {
            get
            {
                if (m_PixelFormatFeature == null)
                    m_PixelFormatFeature = Features ["PixelFormat"];
                return m_PixelFormatFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_PixelFormatFeature = null;

        public long Width
        {
            get { return WidthFeature.IntValue; }
            set { WidthFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature WidthFeature
        {
            get
            {
                if (m_WidthFeature == null)
                    m_WidthFeature = Features ["Width"];
                return m_WidthFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_WidthFeature = null;

        public long WidthMax
        {
            get { return WidthMaxFeature.IntValue; }
        }
        public AVT.VmbAPINET.Feature WidthMaxFeature
        {
            get
            {
                if (m_WidthMaxFeature == null)
                    m_WidthMaxFeature = Features ["WidthMax"];
                return m_WidthMaxFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_WidthMaxFeature = null;

        #endregion

        #region Category /ImageMode

        public long BinningHorizontal
        {
            get { return BinningHorizontalFeature.IntValue; }
            set { BinningHorizontalFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature BinningHorizontalFeature
        {
            get
            {
                if (m_BinningHorizontalFeature == null)
                    m_BinningHorizontalFeature = Features ["BinningHorizontal"];
                return m_BinningHorizontalFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_BinningHorizontalFeature = null;

        public long BinningVertical
        {
            get { return BinningVerticalFeature.IntValue; }
            set { BinningVerticalFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature BinningVerticalFeature
        {
            get
            {
                if (m_BinningVerticalFeature == null)
                    m_BinningVerticalFeature = Features ["BinningVertical"];
                return m_BinningVerticalFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_BinningVerticalFeature = null;

        public long SensorHeight
        {
            get { return SensorHeightFeature.IntValue; }
        }
        public AVT.VmbAPINET.Feature SensorHeightFeature
        {
            get
            {
                if (m_SensorHeightFeature == null)
                    m_SensorHeightFeature = Features ["SensorHeight"];
                return m_SensorHeightFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_SensorHeightFeature = null;

        public long SensorWidth
        {
            get { return SensorWidthFeature.IntValue; }
        }
        public AVT.VmbAPINET.Feature SensorWidthFeature
        {
            get
            {
                if (m_SensorWidthFeature == null)
                    m_SensorWidthFeature = Features ["SensorWidth"];
                return m_SensorWidthFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_SensorWidthFeature = null;

        #endregion

        #region Category /IoControl

        public long StrobeDelay
        {
            get { return StrobeDelayFeature.IntValue; }
            set { StrobeDelayFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature StrobeDelayFeature
        {
            get
            {
                if (m_StrobeDelayFeature == null)
                    m_StrobeDelayFeature = Features ["StrobeDelay"];
                return m_StrobeDelayFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_StrobeDelayFeature = null;

        public long StrobeDuration
        {
            get { return StrobeDurationFeature.IntValue; }
            set { StrobeDurationFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature StrobeDurationFeature
        {
            get
            {
                if (m_StrobeDurationFeature == null)
                    m_StrobeDurationFeature = Features ["StrobeDuration"];
                return m_StrobeDurationFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_StrobeDurationFeature = null;

        public StrobeDurationModeEnum StrobeDurationMode
        {
            get { return (StrobeDurationModeEnum) StrobeDurationModeFeature.EnumIntValue; }
            set { StrobeDurationModeFeature.EnumIntValue = (int) value; }
        }
        public AVT.VmbAPINET.Feature StrobeDurationModeFeature
        {
            get
            {
                if (m_StrobeDurationModeFeature == null)
                    m_StrobeDurationModeFeature = Features ["StrobeDurationMode"];
                return m_StrobeDurationModeFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_StrobeDurationModeFeature = null;

        public StrobeSourceEnum StrobeSource
        {
            get { return (StrobeSourceEnum) StrobeSourceFeature.EnumIntValue; }
            set { StrobeSourceFeature.EnumIntValue = (int) value; }
        }
        public AVT.VmbAPINET.Feature StrobeSourceFeature
        {
            get
            {
                if (m_StrobeSourceFeature == null)
                    m_StrobeSourceFeature = Features ["StrobeSource"];
                return m_StrobeSourceFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_StrobeSourceFeature = null;

        public long SyncInLevels
        {
            get { return SyncInLevelsFeature.IntValue; }
        }
        public AVT.VmbAPINET.Feature SyncInLevelsFeature
        {
            get
            {
                if (m_SyncInLevelsFeature == null)
                    m_SyncInLevelsFeature = Features ["SyncInLevels"];
                return m_SyncInLevelsFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_SyncInLevelsFeature = null;

        public long SyncOutLevels
        {
            get { return SyncOutLevelsFeature.IntValue; }
            set { SyncOutLevelsFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature SyncOutLevelsFeature
        {
            get
            {
                if (m_SyncOutLevelsFeature == null)
                    m_SyncOutLevelsFeature = Features ["SyncOutLevels"];
                return m_SyncOutLevelsFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_SyncOutLevelsFeature = null;

        public SyncOutPolarityEnum SyncOutPolarity
        {
            get { return (SyncOutPolarityEnum) SyncOutPolarityFeature.EnumIntValue; }
            set { SyncOutPolarityFeature.EnumIntValue = (int) value; }
        }
        public AVT.VmbAPINET.Feature SyncOutPolarityFeature
        {
            get
            {
                if (m_SyncOutPolarityFeature == null)
                    m_SyncOutPolarityFeature = Features ["SyncOutPolarity"];
                return m_SyncOutPolarityFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_SyncOutPolarityFeature = null;

        public SyncOutSelectorEnum SyncOutSelector
        {
            get { return (SyncOutSelectorEnum) SyncOutSelectorFeature.EnumIntValue; }
            set { SyncOutSelectorFeature.EnumIntValue = (int) value; }
        }
        public AVT.VmbAPINET.Feature SyncOutSelectorFeature
        {
            get
            {
                if (m_SyncOutSelectorFeature == null)
                    m_SyncOutSelectorFeature = Features ["SyncOutSelector"];
                return m_SyncOutSelectorFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_SyncOutSelectorFeature = null;

        public SyncOutSourceEnum SyncOutSource
        {
            get { return (SyncOutSourceEnum) SyncOutSourceFeature.EnumIntValue; }
            set { SyncOutSourceFeature.EnumIntValue = (int) value; }
        }
        public AVT.VmbAPINET.Feature SyncOutSourceFeature
        {
            get
            {
                if (m_SyncOutSourceFeature == null)
                    m_SyncOutSourceFeature = Features ["SyncOutSource"];
                return m_SyncOutSourceFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_SyncOutSourceFeature = null;

        #endregion

        #region Category /SavedUserSets

        public UserSetDefaultSelectorEnum UserSetDefaultSelector
        {
            get { return (UserSetDefaultSelectorEnum) UserSetDefaultSelectorFeature.EnumIntValue; }
            set { UserSetDefaultSelectorFeature.EnumIntValue = (int) value; }
        }
        public AVT.VmbAPINET.Feature UserSetDefaultSelectorFeature
        {
            get
            {
                if (m_UserSetDefaultSelectorFeature == null)
                    m_UserSetDefaultSelectorFeature = Features ["UserSetDefaultSelector"];
                return m_UserSetDefaultSelectorFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_UserSetDefaultSelectorFeature = null;

        public UserSetSelectorEnum UserSetSelector
        {
            get { return (UserSetSelectorEnum) UserSetSelectorFeature.EnumIntValue; }
            set { UserSetSelectorFeature.EnumIntValue = (int) value; }
        }
        public AVT.VmbAPINET.Feature UserSetSelectorFeature
        {
            get
            {
                if (m_UserSetSelectorFeature == null)
                    m_UserSetSelectorFeature = Features ["UserSetSelector"];
                return m_UserSetSelectorFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_UserSetSelectorFeature = null;

        #endregion

        #region Category /Stream/Info

        public string GVSPFilterVersion
        {
            get { return GVSPFilterVersionFeature.StringValue; }
        }
        public AVT.VmbAPINET.Feature GVSPFilterVersionFeature
        {
            get
            {
                if (m_GVSPFilterVersionFeature == null)
                    m_GVSPFilterVersionFeature = Features ["GVSPFilterVersion"];
                return m_GVSPFilterVersionFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GVSPFilterVersionFeature = null;

        #endregion

        #region Category /Stream/Multicast

        public bool MulticastEnable
        {
            get { return MulticastEnableFeature.BoolValue; }
            set { MulticastEnableFeature.BoolValue = value; }
        }
        public AVT.VmbAPINET.Feature MulticastEnableFeature
        {
            get
            {
                if (m_MulticastEnableFeature == null)
                    m_MulticastEnableFeature = Features ["MulticastEnable"];
                return m_MulticastEnableFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_MulticastEnableFeature = null;

        public long MulticastIPAddress
        {
            get { return MulticastIPAddressFeature.IntValue; }
            set { MulticastIPAddressFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature MulticastIPAddressFeature
        {
            get
            {
                if (m_MulticastIPAddressFeature == null)
                    m_MulticastIPAddressFeature = Features ["MulticastIPAddress"];
                return m_MulticastIPAddressFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_MulticastIPAddressFeature = null;

        #endregion

        #region Category /Stream/Settings

        public long GVSPBurstSize
        {
            get { return GVSPBurstSizeFeature.IntValue; }
            set { GVSPBurstSizeFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature GVSPBurstSizeFeature
        {
            get
            {
                if (m_GVSPBurstSizeFeature == null)
                    m_GVSPBurstSizeFeature = Features ["GVSPBurstSize"];
                return m_GVSPBurstSizeFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GVSPBurstSizeFeature = null;

        public GVSPDriverEnum GVSPDriver
        {
            get { return (GVSPDriverEnum) GVSPDriverFeature.EnumIntValue; }
            set { GVSPDriverFeature.EnumIntValue = (int) value; }
        }
        public AVT.VmbAPINET.Feature GVSPDriverFeature
        {
            get
            {
                if (m_GVSPDriverFeature == null)
                    m_GVSPDriverFeature = Features ["GVSPDriver"];
                return m_GVSPDriverFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GVSPDriverFeature = null;

        public long GVSPHostReceiveBuffers
        {
            get { return GVSPHostReceiveBuffersFeature.IntValue; }
            set { GVSPHostReceiveBuffersFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature GVSPHostReceiveBuffersFeature
        {
            get
            {
                if (m_GVSPHostReceiveBuffersFeature == null)
                    m_GVSPHostReceiveBuffersFeature = Features ["GVSPHostReceiveBuffers"];
                return m_GVSPHostReceiveBuffersFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GVSPHostReceiveBuffersFeature = null;

        public long GVSPMaxLookBack
        {
            get { return GVSPMaxLookBackFeature.IntValue; }
            set { GVSPMaxLookBackFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature GVSPMaxLookBackFeature
        {
            get
            {
                if (m_GVSPMaxLookBackFeature == null)
                    m_GVSPMaxLookBackFeature = Features ["GVSPMaxLookBack"];
                return m_GVSPMaxLookBackFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GVSPMaxLookBackFeature = null;

        public long GVSPMaxRequests
        {
            get { return GVSPMaxRequestsFeature.IntValue; }
            set { GVSPMaxRequestsFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature GVSPMaxRequestsFeature
        {
            get
            {
                if (m_GVSPMaxRequestsFeature == null)
                    m_GVSPMaxRequestsFeature = Features ["GVSPMaxRequests"];
                return m_GVSPMaxRequestsFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GVSPMaxRequestsFeature = null;

        public long GVSPMaxWaitSize
        {
            get { return GVSPMaxWaitSizeFeature.IntValue; }
            set { GVSPMaxWaitSizeFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature GVSPMaxWaitSizeFeature
        {
            get
            {
                if (m_GVSPMaxWaitSizeFeature == null)
                    m_GVSPMaxWaitSizeFeature = Features ["GVSPMaxWaitSize"];
                return m_GVSPMaxWaitSizeFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GVSPMaxWaitSizeFeature = null;

        public long GVSPMissingSize
        {
            get { return GVSPMissingSizeFeature.IntValue; }
            set { GVSPMissingSizeFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature GVSPMissingSizeFeature
        {
            get
            {
                if (m_GVSPMissingSizeFeature == null)
                    m_GVSPMissingSizeFeature = Features ["GVSPMissingSize"];
                return m_GVSPMissingSizeFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GVSPMissingSizeFeature = null;

        public long GVSPPacketSize
        {
            get { return GVSPPacketSizeFeature.IntValue; }
            set { GVSPPacketSizeFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature GVSPPacketSizeFeature
        {
            get
            {
                if (m_GVSPPacketSizeFeature == null)
                    m_GVSPPacketSizeFeature = Features ["GVSPPacketSize"];
                return m_GVSPPacketSizeFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GVSPPacketSizeFeature = null;

        public long GVSPTiltingSize
        {
            get { return GVSPTiltingSizeFeature.IntValue; }
            set { GVSPTiltingSizeFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature GVSPTiltingSizeFeature
        {
            get
            {
                if (m_GVSPTiltingSizeFeature == null)
                    m_GVSPTiltingSizeFeature = Features ["GVSPTiltingSize"];
                return m_GVSPTiltingSizeFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GVSPTiltingSizeFeature = null;

        public long GVSPTimeout
        {
            get { return GVSPTimeoutFeature.IntValue; }
            set { GVSPTimeoutFeature.IntValue = value; }
        }
        public AVT.VmbAPINET.Feature GVSPTimeoutFeature
        {
            get
            {
                if (m_GVSPTimeoutFeature == null)
                    m_GVSPTimeoutFeature = Features ["GVSPTimeout"];
                return m_GVSPTimeoutFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GVSPTimeoutFeature = null;

        #endregion

        #region Category /Stream/Statistics

        public long StatFrameDelivered
        {
            get { return StatFrameDeliveredFeature.IntValue; }
        }
        public AVT.VmbAPINET.Feature StatFrameDeliveredFeature
        {
            get
            {
                if (m_StatFrameDeliveredFeature == null)
                    m_StatFrameDeliveredFeature = Features ["StatFrameDelivered"];
                return m_StatFrameDeliveredFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_StatFrameDeliveredFeature = null;

        public long StatFrameDropped
        {
            get { return StatFrameDroppedFeature.IntValue; }
        }
        public AVT.VmbAPINET.Feature StatFrameDroppedFeature
        {
            get
            {
                if (m_StatFrameDroppedFeature == null)
                    m_StatFrameDroppedFeature = Features ["StatFrameDropped"];
                return m_StatFrameDroppedFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_StatFrameDroppedFeature = null;

        public double StatFrameRate
        {
            get { return StatFrameRateFeature.FloatValue; }
        }
        public AVT.VmbAPINET.Feature StatFrameRateFeature
        {
            get
            {
                if (m_StatFrameRateFeature == null)
                    m_StatFrameRateFeature = Features ["StatFrameRate"];
                return m_StatFrameRateFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_StatFrameRateFeature = null;

        public long StatFrameRescued
        {
            get { return StatFrameRescuedFeature.IntValue; }
        }
        public AVT.VmbAPINET.Feature StatFrameRescuedFeature
        {
            get
            {
                if (m_StatFrameRescuedFeature == null)
                    m_StatFrameRescuedFeature = Features ["StatFrameRescued"];
                return m_StatFrameRescuedFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_StatFrameRescuedFeature = null;

        public long StatFrameShoved
        {
            get { return StatFrameShovedFeature.IntValue; }
        }
        public AVT.VmbAPINET.Feature StatFrameShovedFeature
        {
            get
            {
                if (m_StatFrameShovedFeature == null)
                    m_StatFrameShovedFeature = Features ["StatFrameShoved"];
                return m_StatFrameShovedFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_StatFrameShovedFeature = null;

        public long StatFrameUnderrun
        {
            get { return StatFrameUnderrunFeature.IntValue; }
        }
        public AVT.VmbAPINET.Feature StatFrameUnderrunFeature
        {
            get
            {
                if (m_StatFrameUnderrunFeature == null)
                    m_StatFrameUnderrunFeature = Features ["StatFrameUnderrun"];
                return m_StatFrameUnderrunFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_StatFrameUnderrunFeature = null;

        public double StatLocalRate
        {
            get { return StatLocalRateFeature.FloatValue; }
        }
        public AVT.VmbAPINET.Feature StatLocalRateFeature
        {
            get
            {
                if (m_StatLocalRateFeature == null)
                    m_StatLocalRateFeature = Features ["StatLocalRate"];
                return m_StatLocalRateFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_StatLocalRateFeature = null;

        public long StatPacketErrors
        {
            get { return StatPacketErrorsFeature.IntValue; }
        }
        public AVT.VmbAPINET.Feature StatPacketErrorsFeature
        {
            get
            {
                if (m_StatPacketErrorsFeature == null)
                    m_StatPacketErrorsFeature = Features ["StatPacketErrors"];
                return m_StatPacketErrorsFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_StatPacketErrorsFeature = null;

        public long StatPacketMissed
        {
            get { return StatPacketMissedFeature.IntValue; }
        }
        public AVT.VmbAPINET.Feature StatPacketMissedFeature
        {
            get
            {
                if (m_StatPacketMissedFeature == null)
                    m_StatPacketMissedFeature = Features ["StatPacketMissed"];
                return m_StatPacketMissedFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_StatPacketMissedFeature = null;

        public long StatPacketReceived
        {
            get { return StatPacketReceivedFeature.IntValue; }
        }
        public AVT.VmbAPINET.Feature StatPacketReceivedFeature
        {
            get
            {
                if (m_StatPacketReceivedFeature == null)
                    m_StatPacketReceivedFeature = Features ["StatPacketReceived"];
                return m_StatPacketReceivedFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_StatPacketReceivedFeature = null;

        public long StatPacketRequested
        {
            get { return StatPacketRequestedFeature.IntValue; }
        }
        public AVT.VmbAPINET.Feature StatPacketRequestedFeature
        {
            get
            {
                if (m_StatPacketRequestedFeature == null)
                    m_StatPacketRequestedFeature = Features ["StatPacketRequested"];
                return m_StatPacketRequestedFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_StatPacketRequestedFeature = null;

        public long StatPacketResent
        {
            get { return StatPacketResentFeature.IntValue; }
        }
        public AVT.VmbAPINET.Feature StatPacketResentFeature
        {
            get
            {
                if (m_StatPacketResentFeature == null)
                    m_StatPacketResentFeature = Features ["StatPacketResent"];
                return m_StatPacketResentFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_StatPacketResentFeature = null;

        public double StatTimeElapsed
        {
            get { return StatTimeElapsedFeature.FloatValue; }
        }
        public AVT.VmbAPINET.Feature StatTimeElapsedFeature
        {
            get
            {
                if (m_StatTimeElapsedFeature == null)
                    m_StatTimeElapsedFeature = Features ["StatTimeElapsed"];
                return m_StatTimeElapsedFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_StatTimeElapsedFeature = null;

        #endregion

        #endregion

        #region Public methods.

        #region Category /AcquisitionControl

        public void AcquisitionAbort ()
        {
            AcquisitionAbortFeature.RunCommand ();
        }
        public AVT.VmbAPINET.Feature AcquisitionAbortFeature
        {
            get
            {
                if (m_AcquisitionAbortFeature == null)
                    m_AcquisitionAbortFeature = Features ["AcquisitionAbort"];
                return m_AcquisitionAbortFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_AcquisitionAbortFeature = null;

        public void AcquisitionStart ()
        {
            AcquisitionStartFeature.RunCommand ();
        }
        public AVT.VmbAPINET.Feature AcquisitionStartFeature
        {
            get
            {
                if (m_AcquisitionStartFeature == null)
                    m_AcquisitionStartFeature = Features ["AcquisitionStart"];
                return m_AcquisitionStartFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_AcquisitionStartFeature = null;

        public void AcquisitionStop ()
        {
            AcquisitionStopFeature.RunCommand ();
        }
        public AVT.VmbAPINET.Feature AcquisitionStopFeature
        {
            get
            {
                if (m_AcquisitionStopFeature == null)
                    m_AcquisitionStopFeature = Features ["AcquisitionStop"];
                return m_AcquisitionStopFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_AcquisitionStopFeature = null;

        public void TriggerSoftware ()
        {
            TriggerSoftwareFeature.RunCommand ();
        }
        public AVT.VmbAPINET.Feature TriggerSoftwareFeature
        {
            get
            {
                if (m_TriggerSoftwareFeature == null)
                    m_TriggerSoftwareFeature = Features ["TriggerSoftware"];
                return m_TriggerSoftwareFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_TriggerSoftwareFeature = null;

        #endregion

        #region Category /SavedUserSets

        public void UserSetLoad ()
        {
            UserSetLoadFeature.RunCommand ();
        }
        public AVT.VmbAPINET.Feature UserSetLoadFeature
        {
            get
            {
                if (m_UserSetLoadFeature == null)
                    m_UserSetLoadFeature = Features ["UserSetLoad"];
                return m_UserSetLoadFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_UserSetLoadFeature = null;

        public void UserSetSave ()
        {
            UserSetSaveFeature.RunCommand ();
        }
        public AVT.VmbAPINET.Feature UserSetSaveFeature
        {
            get
            {
                if (m_UserSetSaveFeature == null)
                    m_UserSetSaveFeature = Features ["UserSetSave"];
                return m_UserSetSaveFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_UserSetSaveFeature = null;

        #endregion

        #region Category /Stream/Settings

        public void GVSPAdjustPacketSize ()
        {
            GVSPAdjustPacketSizeFeature.RunCommand ();
        }
        public AVT.VmbAPINET.Feature GVSPAdjustPacketSizeFeature
        {
            get
            {
                if (m_GVSPAdjustPacketSizeFeature == null)
                    m_GVSPAdjustPacketSizeFeature = Features ["GVSPAdjustPacketSize"];
                return m_GVSPAdjustPacketSizeFeature;
            }
        }
        private AVT.VmbAPINET.Feature m_GVSPAdjustPacketSizeFeature = null;

        #endregion

        #endregion

        #region Enum declarations.

        public enum AcquisitionModeEnum
        {
            Continuous = 1,
            SingleFrame = 2,
            MultiFrame = 3,
            Recorder = 4
        }

        public enum BalanceRatioSelectorEnum
        {
            Red = 0,
            Blue = 1
        }

        public enum BalanceWhiteAutoEnum
        {
            Off = 1,
            Continuous = 2,
            Once = 3
        }

        public enum BandwidthControlModeEnum
        {
            StreamBytesPerSecond = 0,
            SCPD = 1,
            Both = 2
        }

        public enum ExposureAutoEnum
        {
            Off = 1,
            Continuous = 2,
            Once = 3
        }

        public enum ExposureAutoAlgEnum
        {
            Mean = 0,
            FitRange = 1
        }

        public enum ExposureModeEnum
        {
            Timed = 1
        }

        public enum GVSPDriverEnum
        {
            Socket = 0,
            Filter = 1
        }

        public enum GainAutoEnum
        {
            Off = 1,
            Continuous = 2,
            Once = 3
        }

        public enum GainSelectorEnum
        {
            All = 0
        }

        public enum GevIPConfigurationModeEnum
        {
            LLA = 4,
            Persistent = 5,
            DHCP = 6
        }

        public enum PixelFormatEnum
        {
            Mono8 = 17301505,
            BayerGR8 = 17301512,
            BayerRG8 = 17301513,
            BayerBG8 = 17301515,
            Mono10 = 17825795,
            Mono12 = 17825797,
            BayerBG10 = 17825807,
            BayerGR12 = 17825808,
            BayerRG12 = 17825809,
            YUV411Packed = 34340894,
            YUV422Packed = 34603039,
            RGB8Packed = 35127316,
            BGR8Packed = 35127317,
            YUV444Packed = 35127328,
            RGBA8Packed = 35651606,
            BGRA8Packed = 35651607,
            RGB10Packed = 36700184,
            RGB12Packed = 36700186
        }

        public enum StreamHoldEnableEnum
        {
            Off = 0,
            On = 1
        }

        public enum StrobeDurationModeEnum
        {
            Source = 0,
            Controlled = 1
        }

        public enum StrobeSourceEnum
        {
            AcquisitionTriggerReady = 1,
            FrameTriggerReady = 2,
            FrameTrigger = 3,
            Exposing = 4,
            FrameReadout = 5,
            LineIn1 = 8,
            LineIn2 = 9
        }

        public enum SyncOutPolarityEnum
        {
            Normal = 0,
            Invert = 1
        }

        public enum SyncOutSelectorEnum
        {
            SyncOut1 = 0,
            SyncOut2 = 1,
            SyncOut3 = 2
        }

        public enum SyncOutSourceEnum
        {
            GPO = 0,
            AcquisitionTriggerReady = 1,
            FrameTriggerReady = 2,
            FrameTrigger = 3,
            Exposing = 4,
            FrameReadout = 5,
            Imaging = 6,
            Acquiring = 7,
            LineIn1 = 8,
            LineIn2 = 9,
            Strobe1 = 12
        }

        public enum TriggerActivationEnum
        {
            RisingEdge = 0,
            FallingEdge = 1,
            AnyEdge = 2,
            LevelHigh = 3,
            LevelLow = 4
        }

        public enum TriggerModeEnum
        {
            Off = 0,
            On = 1
        }

        public enum TriggerSelectorEnum
        {
            FrameStart = 0,
            AcquisitionStart = 3,
            AcquisitionEnd = 4,
            AcquisitionRecord = 6
        }

        public enum TriggerSourceEnum
        {
            Freerun = 0,
            Line1 = 1,
            Line2 = 2,
            Line3 = 3,
            Line4 = 4,
            FixedRate = 5,
            Software = 6
        }

        public enum UserSetDefaultSelectorEnum
        {
            Default = 0,
            UserSet1 = 1,
            UserSet2 = 2,
            UserSet3 = 3,
            UserSet4 = 4,
            UserSet5 = 5
        }

        public enum UserSetSelectorEnum
        {
            Default = 0,
            UserSet1 = 1,
            UserSet2 = 2,
            UserSet3 = 3,
            UserSet4 = 4,
            UserSet5 = 5
        }

        #endregion
    }
}
